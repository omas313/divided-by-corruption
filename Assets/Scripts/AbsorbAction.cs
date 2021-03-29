using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorbAction : BattleAction, IActionBarAction
{
    public override ActionDefinition ActionDefinition => AbsorbDefinition;
    public override bool IsValid => Performer != null && Targets != null;

    public ActionBarResult ActionBarResult { get; set; }
    public AbsorbDefinition AbsorbDefinition { get; set; }
    public List<SegmentData> SegmentData => _segmentData;

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

    protected override IEnumerator Perform(List<PartyMember> party, List<Enemy> enemies)
    {
        Debug.Log("absorbing");
        yield return new WaitForSeconds(1f);
    }
}