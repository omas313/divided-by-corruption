using UnityEngine;

[System.Serializable]
public class SegmentData
{
    public bool HasNormalArea => _normalMultiplier != 0f && _normalSubSegmentSize != 0f;
    public bool HasCriticalArea => _criticalMultiplier != 0f && _criticalSubSegmentSize != 0f;
    public float NormalMultiplier => _normalMultiplier;
    public float CriticalMultiplier => _criticalMultiplier;
    public float NormalSubSegmentWidth => _normalSubSegmentSize;
    public float CriticalSubSegmentWidth => _criticalSubSegmentSize;
    public UIAttackBarSegment UIAttackBarSegmentPrefab => _segmentType.Prefab;

    [SerializeField] float _normalMultiplier = 1f;
    [SerializeField] float _normalSubSegmentSize = 80f;
    [SerializeField] float _criticalMultiplier = 1.2f;
    [SerializeField] float _criticalSubSegmentSize = 20f;
    [SerializeField] SegmentType _segmentType;

    public SegmentData(float normalMultiplier, float criticalMultiplier)
    {
        _normalMultiplier = normalMultiplier;
        _criticalMultiplier = criticalMultiplier;
    }
}
