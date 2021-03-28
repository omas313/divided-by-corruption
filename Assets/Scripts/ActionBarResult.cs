using System.Collections.Generic;

public class ActionBarResult
{
    public List<SegmentResult> SegmentsResults;

    public ActionBarResult(List<SegmentResult> segmentsResults)
    {
        SegmentsResults = segmentsResults;
    }
}
