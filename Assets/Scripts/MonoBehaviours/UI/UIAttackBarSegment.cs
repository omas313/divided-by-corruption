using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAttackBarSegment : MonoBehaviour
{
    public bool IsActive { get; private set; }
    public Area Area { get; private set; }
    public Area NormalArea { get; private set; }
    public Area CriticalArea { get; private set; }
    public float AnchoredPosition => _rectTransform.anchoredPosition.x;
    public float NormalMultiplier => _normalMultiplier;
    public float CriticalMultiplier => _criticalMultiplier;

    [SerializeField] float _normalMultiplier = 1f;
    [SerializeField] float _criticalMultiplier = 1.25f;
    [SerializeField] RectTransform _normalArea;
    [SerializeField] RectTransform _criticalArea;
    [SerializeField] Image _overlayImage;

    RectTransform _rectTransform;

    public void Init(float xPosition)
    {
        _rectTransform.anchoredPosition = new Vector2(xPosition, 0f);

        Area = new Area(_rectTransform.anchoredPosition.x, _rectTransform.anchoredPosition.x + _rectTransform.sizeDelta.x);

        NormalArea = new Area(
            _rectTransform.anchoredPosition.x + _normalArea.anchoredPosition.x, 
            _rectTransform.anchoredPosition.x + _normalArea.anchoredPosition.x + _normalArea.sizeDelta.x);

        CriticalArea = new Area(
            _rectTransform.anchoredPosition.x + _criticalArea.anchoredPosition.x, 
            _rectTransform.anchoredPosition.x + _criticalArea.anchoredPosition.x + _criticalArea.sizeDelta.x);

        SetActive(true);
    }

    public bool IsInside(float position) => Area.IsInside(position);

    public float GetMultiplier(float position)
    {
        if (NormalArea.IsInside(position))
            return NormalMultiplier;
        else if (CriticalArea.IsInside(position))
            return CriticalMultiplier;
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
        _rectTransform = GetComponent<RectTransform>();    
    }
}
