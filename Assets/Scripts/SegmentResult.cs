public class SegmentResult
{
    public SegmentData SegmentData { get; private set; }
    public bool IsMiss { get; private set; }
    public bool IsCritical { get; private set; }
    public float Multiplier { get; private set; }
    public bool IsHit => !IsMiss;

    public SegmentResult(SegmentData segmentData, float multiplier, bool isCritical = false, bool isMiss = false)
    {
        SegmentData = segmentData;
        Multiplier = multiplier;
        IsCritical = isCritical;
        IsMiss = isMiss;
    }
}