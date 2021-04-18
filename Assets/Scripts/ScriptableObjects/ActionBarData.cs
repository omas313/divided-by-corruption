using System.Collections.Generic;

public class ActionBarData
{
    public List<SegmentData> SegmentsData { get; set; }
    public SegmentModifier NormalSegmentModifier { get; set; }
    public SegmentModifier CriticalSegmentModifier { get; set; }
}
