using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorbAction : BattleAction, IActionBarAction
{
    public override ActionDefinition ActionDefinition => AbsorbDefinition;
    public override bool IsValid => 
        Performer != null 
        && Targets != null 
        && Targets.Count > 0
        && ActionBarResult != null;
    public override bool HasFailed { get; protected set; }

    public ActionBarResult ActionBarResult { get; set; }
    public AbsorbDefinition AbsorbDefinition { get; set; }
    public List<SegmentData> SegmentData => _segmentData;

    List<SegmentData> _segmentData;

    public AbsorbAction(BattleParticipant performer, AbsorbDefinition absorbDefinition)
    {
        BattleActionType = BattleActionType.Absorb;
        Performer = performer;
        AbsorbDefinition = absorbDefinition;

        _segmentData = new List<SegmentData>() 
        { 
            new SegmentData(0.8f, 1f)
        };
    }

    protected override IEnumerator Perform(List<PartyMember> party, List<Enemy> enemies)
    {
        if (ActionBarResult.SegmentsResults[0].IsMiss)
        {
            Debug.Log("absorb failed");
            yield return new WaitForSeconds(0.5f);
            yield break;
        }

        BattleEvents.InvokeBattleParticipantsTargetted(Targets);

        yield return AbsorbDefinition.SpawnEffect(Targets[0].BodyMidPointPosition, Performer.BodyMidPointPosition);

        var amountToAbsorb = Mathf.CeilToInt(ActionBarResult.SegmentsResults[0].Multiplier * AbsorbDefinition.AbsorbtionPercentage);
        var amountAbsorbed = 0;
        foreach (var target in Targets)
            amountAbsorbed += target.TakeMP(amountToAbsorb);

        Performer.AddMP(amountAbsorbed);
        BattleEvents.InvokeMPAbsorbed(Performer, Targets[0], amountAbsorbed);
        yield return new WaitForSeconds(0.25f);
    }
}
