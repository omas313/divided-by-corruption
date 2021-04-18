using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAttackBarSegment : MonoBehaviour
{
    public bool IsActive { get; private set; }
    public bool HasNormalArea => _normalAreaRect != null;
    public bool HasCriticalArea => _criticalAreaRect != null;
    public Area Area { get; private set; }
    public Area NormalArea { get; private set; }
    public Area CriticalArea { get; private set; }
    public float AnchoredPosition => _mainRectTransform.anchoredPosition.x;

    [SerializeField] float _normalMultiplier = 1f;
    [SerializeField] float _criticalMultiplier = 1.25f;
    [SerializeField] RectTransform _normalAreaRect;
    [SerializeField] RectTransform _criticalAreaRect;
    [SerializeField] Image _overlayImage;
    [SerializeField] bool _criticalAreaFirst;

    RectTransform _mainRectTransform;
    SegmentModifier _normalSegmentModifier;
    SegmentModifier _criticalSegmentModifier;
    float _mainHeight;

    public void Init(SegmentData data, SegmentModifier normalSegmentModifier, SegmentModifier criticalSegmentModifier, float mainXPosition)
    {
        _normalSegmentModifier = normalSegmentModifier;
        _criticalSegmentModifier = criticalSegmentModifier;
        var normalWidth = data.NormalSubSegmentWidth * normalSegmentModifier.Width;
        var criticalWidth = data.CriticalSubSegmentWidth * criticalSegmentModifier.Width;
        var mainWidth = normalWidth + criticalWidth;

        _mainRectTransform.anchoredPosition = new Vector2(mainXPosition, 0f);
        _mainRectTransform.sizeDelta = new Vector2(mainWidth, _mainHeight);

        if (HasNormalArea && data.HasNormalArea)
        {
            var normalXPosition = 0f; // for now, assuming normal is first

            _normalAreaRect.anchoredPosition = new Vector2(normalXPosition, 0f);
            _normalAreaRect.sizeDelta = new Vector2(normalWidth, _mainHeight);

            NormalArea = new Area(
                mainXPosition + normalXPosition, 
                mainXPosition + normalXPosition + normalWidth);
        }

        if (HasCriticalArea && data.HasCriticalArea)
        {
            // for now, assuming normal is first
            var normalXPosition = HasNormalArea ? _normalAreaRect.anchoredPosition.x : 0f; 
            var criticalXPosition = normalXPosition + (HasNormalArea ? normalWidth : 0f);

            _criticalAreaRect.anchoredPosition = new Vector2(criticalXPosition, 0f);
            _criticalAreaRect.sizeDelta = new Vector2(criticalWidth, _mainHeight);

            CriticalArea = new Area(
                mainXPosition + criticalXPosition,
                mainXPosition + criticalXPosition + criticalWidth);
        }
        
        Area = new Area(mainXPosition, mainXPosition + mainWidth);

        // Debug.Log($"TTL AREA: [{Area.Start}, {Area.End}]");
        // Debug.Log($"NORMAL AREA: [{NormalArea.Start}, {NormalArea.End}]");
        // Debug.Log($"CRITICAL AREA: [{CriticalArea.Start}, {CriticalArea.End}]");
        // Debug.Log($"RECT SIZE DELTA: {_mainRectTransform.sizeDelta}");

        SetActive(true);
    }

    public bool IsInside(float position) => Area.IsInside(position);
    public bool IsInsideNormalArea(float position) => HasNormalArea ? NormalArea.IsInside(position) : false;
    public bool IsInsideCriticalArea(float position) => HasCriticalArea ? CriticalArea.IsInside(position) : false;

    public float GetMultiplier(float position)
    {
        if (HasNormalArea && NormalArea.IsInside(position))
            return _normalMultiplier * _normalSegmentModifier.Value;
        else if (HasCriticalArea && CriticalArea.IsInside(position))
            return _criticalMultiplier * _criticalSegmentModifier.Value;
        else 
            return 0f;
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        _overlayImage.enabled = !isActive;
    }

    void Awake()
    {
        _mainRectTransform = GetComponent<RectTransform>();    
        _mainHeight = _mainRectTransform.sizeDelta.y;

        if (!HasCriticalArea && !HasNormalArea)
            Debug.Log($"Error in segment {name}: No normal or critical area specified (serialized field)");
    }
}
