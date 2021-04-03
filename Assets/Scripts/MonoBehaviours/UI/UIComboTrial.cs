using System.Collections;
using UnityEngine;
using TMPro;

public class UIComboTrial : MonoBehaviour
{
    [SerializeField] RectTransform _pin;
    [SerializeField] float _pinSpeed = 300f;
    [SerializeField] Transform _textsParent;
    [SerializeField] GameObject _textPrefab;
    [SerializeField] Animation _titleAnimation;

    UIComboTrialSegment[] _segments;
    RectTransform _rectTransform;
    CanvasGroup _canvasGroup;
    bool _isMoving;
    float _totalWidth;
    bool _confirmed;
    BattleActionPacket _currentBattleActionPacket;

    void Hide() => _canvasGroup.alpha = 0f;

    void Show() => _canvasGroup.alpha = 1f;

    void Init()
    {
        _confirmed = false;
        _pin.anchoredPosition = Vector3.zero;

        var correctIndex = UnityEngine.Random.Range(0, _segments.Length);
        for (var i = 0; i < _segments.Length; i++)
            _segments[i].SetValue(i == correctIndex);
    }

    IEnumerator StartTrial()
    {
        Show();
        yield return PlayTitleAnimation();
        yield return PlaySegmentsAnimations();
        StartCoroutine(MovePin());
    }

    IEnumerator PlayTitleAnimation()
    {
        _titleAnimation.Play();
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !_titleAnimation.isPlaying);
    }

    IEnumerator PlaySegmentsAnimations()
    {
        foreach (var segment in _segments)
            StartCoroutine(segment.PlayAnimation());

        yield return new WaitUntil(() => 
        {
            foreach (var segment in _segments)
                if (!segment.IsAnimationDone)
                    return false;

            return true;
        });
    }

    IEnumerator MovePin()
    {
        _isMoving = true;
        
        while (_pin.anchoredPosition.x < _totalWidth)
        {
            _pin.anchoredPosition = new Vector3(_pin.anchoredPosition.x + Time.deltaTime * _pinSpeed, 0f, 0f);    

            if (_confirmed)
                break;

            yield return null;
        }

        if (!_confirmed)
            TrialFailed();

        foreach (var segment in _segments)
            segment.RevealType();
            
        yield return new WaitForSeconds(2f);
        Hide();
        
        (_currentBattleActionPacket.BattleAction as ComboTrialAction).SetResult(_confirmed);
        BattleUIEvents.InvokeComboTrialCompleted();

        CleanUp();
        _isMoving = false;
    }

    void TrialSucceeded(UIComboTrialSegment segment)
    {
        CreateText("success", Color.yellow, 1.1f);
    }

    void TrialFailed()
    {
        CreateText("failure", Color.red);
        // BattleUIEvents.InvokeComboTrialFailed(); // todo: remove the combo indicators
    }

    void Confirm()
    {
        _confirmed = true;
        var pinPositionX = _pin.anchoredPosition.x;

        foreach (var segment in _segments)
            if (segment.IsInside(pinPositionX) && segment.IsCorrect)
            {
                TrialSucceeded(segment);
                return;
            }
        
        TrialFailed();
    }

    void CleanUp()
    {
        foreach (var text in _textsParent.GetComponentsInChildren<Animation>())
            Destroy(text.gameObject);
    }


    void CreateText(string text, Color color, float scale = 1f)
    {
        var textMesh = Instantiate(_textPrefab, _pin.position, Quaternion.identity, _textsParent).GetComponentInChildren<TextMeshProUGUI>();
        textMesh.SetText(text);
        textMesh.color = color;
        textMesh.transform.localScale = new Vector3(scale, scale, scale);
    }
    
    void OnComboTrialRequested(BattleActionPacket battleActionPacket)
    {
        _currentBattleActionPacket = battleActionPacket;
        Init();
        StartCoroutine(StartTrial());
    }

    void Update()
    {
        if (Input.GetButtonDown("Confirm") && _isMoving)
            Confirm();
    }

    void OnDestroy()
    {
        BattleEvents.ComboTrialRequested -= OnComboTrialRequested;
    }

    void Awake()
    {
        _segments = GetComponentsInChildren<UIComboTrialSegment>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
        _totalWidth = _rectTransform.sizeDelta.x;

        Hide();

        BattleEvents.ComboTrialRequested += OnComboTrialRequested;
    }
}
