using System.Collections.Generic;

public interface IActionBarAction
{
    ActionBarResult ActionBarResult { get; set; }
    ActionBarData ActionBarData { get; set; }
}
