using UnityEngine;

[System.Serializable]
public class SegmentData
{
    public float NormalMultiplier => _normalMultiplier;
    public float CriticalMultiplier => _criticalMultiplier;

    [SerializeField] float _normalMultiplier = 1f;
    [SerializeField] float _criticalMultiplier = 1.2f;

    public SegmentData(float normalMultiplier, float criticalMultiplier)
    {
        _normalMultiplier = normalMultiplier;
        _criticalMultiplier = criticalMultiplier;
    }
}