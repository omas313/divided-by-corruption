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
    [SerializeField] float _yPositionOffset = 100f;

    List<UIAttackBarSegment> _uiSegments;
    Dictionary<UIAttackBarSegment, SegmentData> _uiSegmentsDataMap;
    CanvasGroup _canvasGroup;
    RectTransform _rectTransform;
    bool _isMoving;
    float _totalWidth;
    bool _isNextTextPositionIsDown;
    List<SegmentResult> _currentSegmentResults;
    BattleActionPacket _currentBattleActionPacket;
    IActionBarAction CurrentBattleAction => _currentBattleActionPacket.BattleAction as IActionBarAction;

    bool _hasMissedConfirm;

    void Hide() => _canvasGroup.alpha = 0f;

    void Show() => _canvasGroup.alpha = 1f;

    void StartWithSegments(List<SegmentData> segmentsData, float pinSpeed)
    {
        if (segmentsData.Count > MAX_SEGMENT_COUNT)
            Debug.Log($"Error: received {segmentsData.Count} segment count, where max is {MAX_SEGMENT_COUNT}");

        CreateSegments(segmentsData);
        Show();
        StartCoroutine(StartOperation(pinSpeed));
    }

    void CreateSegments(List<SegmentData> segmentsData)
    {
        _uiSegments = new List<UIAttackBarSegment>();

        for (int i = 0; i < segmentsData.Count; i++)
        {
            var data = segmentsData[i];
            var xPosition = (_firstSegmentPosition + _segmentSpacing * i) % _totalWidth;
            var segment = Instantiate(data.UIAttackBarSegmentPrefab, Vector3.zero, Quaternion.identity, _segmentsParent);
            segment.Init(data, xPosition);
            _uiSegments.Add(segment);
        }

        _uiSegments = _uiSegments.OrderBy(s => s.AnchoredPosition).ToList();

        _uiSegmentsDataMap = new Dictionary<UIAttackBarSegment, SegmentData>();
        for (int i = 0; i < segmentsData.Count; i++)
            _uiSegmentsDataMap[_uiSegments[i]] = segmentsData[i];

        _currentSegmentResults = new List<SegmentResult>();
    }

    IEnumerator StartOperation(float pinSpeed)
    {
        _isMoving = true;
        _hasMissedConfirm = false;

        _pin.anchoredPosition = Vector3.zero;
        
        while (_pin.anchoredPosition.x < _totalWidth)
        {
            _pin.anchoredPosition = new Vector3(_pin.anchoredPosition.x + Time.deltaTime * pinSpeed, 0f, 0f);    

            if (HasEnoughResults() || HasMissedASegment() || _hasMissedConfirm)
                break;

            yield return null;
        }

        _isMoving = false;
        yield return new WaitForSeconds(2f);

        Hide();
        CurrentBattleAction.ActionBarResult = new ActionBarResult(_currentSegmentResults);
        BattleUIEvents.InvokeActionBarCompleted();
        CleanUp();
    }

    bool HasMissedASegment()
    {
        foreach (var uiSegment in _uiSegments)
            if (_pin.anchoredPosition.x > uiSegment.Area.End && uiSegment.IsActive)
            {
                uiSegment.SetActive(false);
                CreateMiss(uiSegment);
                return true;
            }

        return false;
    }

    void ConfirmMissed()
    {
        foreach (var uiSegment in _uiSegments)
        {
            if (uiSegment.IsActive)
            {
                uiSegment.SetActive(false);
                CreateConfirmPoint();
                CreateMiss(uiSegment);
                _hasMissedConfirm = true;
                break;
            }
        }
    }

    bool HasEnoughResults() => _currentSegmentResults.Count == _uiSegments.Count;

    void CreateMiss(UIAttackBarSegment uiSegment)
    {
        CreateText("miss", Color.gray);
        _currentSegmentResults.Add(new SegmentResult(_uiSegmentsDataMap[uiSegment], 0f, false, true));
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
            CreateConfirmPoint();

            if (uiSegment.IsInsideNormalArea(pinPositionX))
            {
                _currentSegmentResults.Add(new SegmentResult(_uiSegmentsDataMap[uiSegment], _uiSegmentsDataMap[uiSegment].NormalMultiplier));
                CreateText("hit", Color.white);
                return;
            }
            else if (uiSegment.IsInsideCriticalArea(pinPositionX))
            {
                _currentSegmentResults.Add(new SegmentResult(_uiSegmentsDataMap[uiSegment], _uiSegmentsDataMap[uiSegment].CriticalMultiplier, isCritical: true));
                CreateText("critical!", Color.red, scale: 1.1f);
                return;
            }
        }
        
        ConfirmMissed();
    }

    void CreateConfirmPoint() => Instantiate(_confirmPointPrefab, _pin.position, Quaternion.identity, _confirmPointsParent);

    void CreateText(string text, Color color, float scale = 1f)
    {
        var position = _pin.position + new Vector3(0f, _isNextTextPositionIsDown ? -_yPositionOffset : _yPositionOffset, 0f);
        _isNextTextPositionIsDown = !_isNextTextPositionIsDown;

        var textMesh = Instantiate(_textPrefab, position , Quaternion.identity, _textsParent).GetComponentInChildren<TextMeshProUGUI>();
        textMesh.SetText(text);
        textMesh.color = color;
        textMesh.transform.localScale = new Vector3(scale, scale, scale);
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
