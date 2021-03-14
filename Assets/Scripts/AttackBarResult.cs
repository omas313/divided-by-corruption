using System.Collections.Generic;

public class AttackBarResult
{
    public List<SegmentResult> SegmentsResults;

    public AttackBarResult(List<SegmentResult> segmentsResults)
    {
        SegmentsResults = segmentsResults;
    }
}
