using System.Collections;
using System.Collections.Generic;

public class AbsorbAction : BattleAction, IActionBarAction
{
    public ActionBarResult ActionBarResult { get; set; }
    public List<SegmentData> SegmentData => _segmentData;
    public override bool IsValid => Performer != null && Target != null;

    List<SegmentData> _segmentData;

    public AbsorbAction(BattleParticipant performer)
    {
        BattleActionType = BattleActionType.Absorb;
        Performer = performer;

        _segmentData = new List<SegmentData>() 
        { 
            new SegmentData(0.8f, 1f)
        };
    }

    public override IEnumerator Perform(List<PartyMember> party, List<Enemy> enemies)
    {
        throw new System.NotImplementedException();
    }
}