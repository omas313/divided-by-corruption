using System.Collections.Generic;

public interface IActionBarAction
{
    ActionBarResult ActionBarResult { get; set; }
    List<SegmentData> SegmentData { get; }
}
