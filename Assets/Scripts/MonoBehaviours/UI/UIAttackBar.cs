using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class UIAttackBar : MonoBehaviour
{
    const int MAX_SEGMENT_COUNT = 4;
    [SerializeField] RectTransform _pin;
    [SerializeField] float _pinSpeed = 20f;

    [SerializeField] GameObject _confirmPointPrefab;
    [SerializeField] Transform _confirmPointsParent;

    [SerializeField] RectTransform _segmentsParent;
    [SerializeField] UIAttackBarSegment _segmentPrefab;
    [SerializeField] float _segmentSpacing = 150f;
    [SerializeField] float _firstSegmentPosition = 200f;

    [SerializeField] Transform _textsParent;
    [SerializeField] GameObject _textPrefab;

    List<UIAttackBarSegment> _uiSegments;
    Dictionary<UIAttackBarSegment, SegmentData> _uiSegmentsDataMap;
    CanvasGroup _canvasGroup;
    RectTransform _rectTransform;
    bool _isMoving;
    float _totalWidth;
    List<SegmentResult> _currentSegmentResults;
    BattleActionPacket _currentBattleActionPacket;
    IActionBarAction CurrentBattleAction => _currentBattleActionPacket.BattleAction as IActionBarAction;

    void Hide() => _canvasGroup.alpha = 0f;

    void Show() => _canvasGroup.alpha = 1f;

    void StartWithSegments(List<SegmentData> segments, float pinSpeed)
    {
        if (segments.Count > MAX_SEGMENT_COUNT)
            Debug.Log($"Error: received {segments.Count} segment count, where max is {MAX_SEGMENT_COUNT}");

        CreateSegments(segments);
        Show();
        StartCoroutine(StartOperation(pinSpeed));
    }

    void CreateSegments(List<SegmentData> segments)
    {
        _uiSegments = new List<UIAttackBarSegment>();

        for (int i = 0; i < segments.Count; i++)
        {
            var xPosition = (_firstSegmentPosition + _segmentSpacing * i) % _totalWidth;
            var segment = Instantiate(_segmentPrefab, Vector3.zero, Quaternion.identity, _segmentsParent);
            segment.Init(xPosition);
            _uiSegments.Add(segment);
        }

        _uiSegments = _uiSegments.OrderBy(s => s.AnchoredPosition).ToList();

        _uiSegmentsDataMap = new Dictionary<UIAttackBarSegment, SegmentData>();
        for (int i = 0; i < segments.Count; i++)
            _uiSegmentsDataMap[_uiSegments[i]] = segments[i];

        _currentSegmentResults = new List<SegmentResult>();
    }

    IEnumerator StartOperation(float pinSpeed)
    {
        _isMoving = true;

        _pin.anchoredPosition = Vector3.zero;
        
        while (_pin.anchoredPosition.x < _totalWidth)
        {
            _pin.anchoredPosition = new Vector3(_pin.anchoredPosition.x + Time.deltaTime * pinSpeed, 0f, 0f);    

            DeactivatePassedSegments();
            
            if (HasEnoughResults())
                break;

            yield return null;
        }

        yield return new WaitForSeconds(2f);

        Hide();
        CurrentBattleAction.ActionBarResult = new ActionBarResult(_currentSegmentResults);
        BattleUIEvents.InvokeActionBarCompleted();
            
        CleanUp();
        _isMoving = false;
    }

    bool HasEnoughResults() => _currentSegmentResults.Count == _uiSegments.Count;

    void DeactivatePassedSegments()
    {
        foreach (var uiSegment in _uiSegments)
            if (_pin.anchoredPosition.x > uiSegment.Area.End && uiSegment.IsActive)
            {
                uiSegment.SetActive(false);
                CreateText("miss", Color.gray);
                _currentSegmentResults.Add(new SegmentResult(_uiSegmentsDataMap[uiSegment], 0f, false, true));
            }
    }

    void CleanUp()
    {
        foreach (var confirmPoint in _confirmPointsParent.GetComponentsInChildren<UIConfirmPoint>())
            Destroy(confirmPoint.gameObject);
        foreach (var text in _textsParent.GetComponentsInChildren<Animation>())
            Destroy(text.gameObject);
        foreach (var segment in _uiSegments)
            Destroy(segment.gameObject);
    }

    void Confirm()
    {
        var pinPositionX = _pin.anchoredPosition.x;

        foreach (var uiSegment in _uiSegments)
        {
            if (!uiSegment.IsActive)
                continue;

            if (!uiSegment.IsInside(pinPositionX))
                continue;

            uiSegment.SetActive(false);
            Instantiate(_confirmPointPrefab, _pin.position, Quaternion.identity, _confirmPointsParent);

            if (uiSegment.NormalArea.IsInside(pinPositionX))
            {
                _currentSegmentResults.Add(new SegmentResult(_uiSegmentsDataMap[uiSegment], _uiSegmentsDataMap[uiSegment].NormalMultiplier));
                CreateText("hit", Color.white);
                return;
            }
            else if (uiSegment.CriticalArea.IsInside(pinPositionX))
            {
                _currentSegmentResults.Add(new SegmentResult(_uiSegmentsDataMap[uiSegment], _uiSegmentsDataMap[uiSegment].CriticalMultiplier, isCritical: true));
                CreateText("critical", Color.red, scale: 1.1f);
                return;
            }
        }
        
        CreateMissAtNextActiveSegment();
    }

    void CreateMissAtNextActiveSegment()
    {
        foreach (var uiSegment in _uiSegments)
        {
            if (uiSegment.IsActive)
            {
                uiSegment.SetActive(false);

                Instantiate(_confirmPointPrefab, _pin.position, Quaternion.identity, _confirmPointsParent);
                CreateText("miss", Color.gray);
                _currentSegmentResults.Add(new SegmentResult(_uiSegmentsDataMap[uiSegment], 0f, false, true));
                break;
            }
        }
    }

    void CreateText(string text, Color color, float scale = 1f)
    {
        var textMesh = Instantiate(_textPrefab, _pin.position, Quaternion.identity, _textsParent).GetComponentInChildren<TextMeshProUGUI>();
        textMesh.SetText(text);
        textMesh.color = color;
        textMesh.transform.localScale = new Vector3(scale, scale, scale);
    }

    void OnPartyMemberTurnStarted(PartyMember partyMember, BattleActionPacket battleActionPacket)
    {
        _currentBattleActionPacket = battleActionPacket;
    }
    
    void OnPartyMemberTurnEnded(PartyMember partyMember)
    {
        _currentBattleActionPacket = null;
    }

    void OnRequestedActionBar()
    {
        Show();
        StartWithSegments(CurrentBattleAction.SegmentData, _pinSpeed);
    }

    void Update()
    {
        if (Input.GetButtonDown("Confirm") && _isMoving)
            Confirm();
    }

    void OnDestroy()
    {
        BattleUIEvents.ActionBarRequested -= OnRequestedActionBar;
        BattleEvents.PartyMemberTurnStarted -= OnPartyMemberTurnStarted;
        BattleEvents.PartyMemberTurnEnded -= OnPartyMemberTurnEnded;
    }

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
        _totalWidth = _rectTransform.sizeDelta.x;

        Hide();

        BattleEvents.PartyMemberTurnStarted += OnPartyMemberTurnStarted;
        BattleEvents.PartyMemberTurnEnded += OnPartyMemberTurnEnded;
        BattleUIEvents.ActionBarRequested += OnRequestedActionBar;
    }
}
