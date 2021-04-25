using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public ActionBarData ActionBarData {get; set; }

    public AbsorbAction(BattleParticipant performer, AbsorbDefinition absorbDefinition)
    {
        BattleActionType = BattleActionType.Absorb;
        Performer = performer;
        AbsorbDefinition = absorbDefinition;
        ActionBarData = new ActionBarData
        {
            SegmentsData = absorbDefinition.SegmentsData,
            NormalSegmentModifier = new SegmentModifier(),
            CriticalSegmentModifier = new SegmentModifier()
        };
    }

    protected override IEnumerator Perform(List<PartyMember> party, List<Enemy> enemies)
    {
        if (ActionBarResult.SegmentsResults[0].IsMiss)
        {
            yield return new WaitForSeconds(0.5f);
            yield break;
        }

        BattleEvents.InvokeBattleParticipantsTargetted(Targets);
        yield return AbsorbDefinition.SpawnEffect(Targets[0].BodyMidPointPosition, Performer.BodyMidPointPosition);
        AbsorbMP();
        TryToLearnComboEffectModifier();
        yield return new WaitForSeconds(0.25f);
    }

    private void AbsorbMP()
    {
        var amountToAbsorb = Mathf.CeilToInt(ActionBarResult.SegmentsResults[0].Multiplier * AbsorbDefinition.AbsorbtionPercentage);
        var amountAbsorbed = 0;
        foreach (var target in Targets)
            amountAbsorbed += target.TakeMP(amountToAbsorb);

        Performer.AddMP(amountAbsorbed);
        BattleEvents.InvokeMPAbsorbed(Performer, Targets[0], amountAbsorbed);
    }

    void TryToLearnComboEffectModifier()
    {
        var randomEnemy = Targets[UnityEngine.Random.Range(0, Targets.Count)] as Enemy;
        var partyMember = Performer as PartyMember;
        var teachableEffectModifiers = randomEnemy.Definition.EffectDefinitions
            .Where(em => partyMember.CanLearnEffectModifier(em))
            .ToList();
        
        if (teachableEffectModifiers.Count <= 0)
            return;

        var randomEffectModifier = teachableEffectModifiers[UnityEngine.Random.Range(0, teachableEffectModifiers.Count)];
        partyMember.LearnEffectModifier(randomEffectModifier);
        BattleEvents.InvokeEffectLearnt(partyMember, randomEffectModifier);
    }
}
