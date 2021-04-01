using System;
using System.Collections;
using System.Collections.Generic;

public class ComboTrialAction : BattleAction
{
    public override ActionDefinition ActionDefinition => ComboTrialDefinition;
    public override bool IsValid => 
        Performer != null 
        && Targets != null 
        && Targets.Count > 0
        && HasResult;

    public ComboTrialDefinition ComboTrialDefinition { get; set; }
    public bool HasResult { get; private set; }
    public bool IsSuccess { get; private set; }
    public AttackDefinition AttackDefinition { get; private set; }

    public ComboTrialAction(PartyMember starter, PartyMember ender, AttackDefinition attackDefinition)
    {
        Performer = starter;
        Targets.Add(ender);
        AttackDefinition = attackDefinition;
    }

    public void SetResult(bool result)
    {
        HasResult = true;
        IsSuccess = result;
    }

    protected override IEnumerator Perform(List<PartyMember> party, List<Enemy> enemies)
    {
        if (!IsSuccess)
            yield break;

        var comboEnder = Targets[0] as PartyMember;
        foreach (var effectDefinition in AttackDefinition.ComboEffectDefinitions)
            comboEnder.EffectsManager.AddEffect(new Effect(effectDefinition, comboEnder));
    }

    protected override IEnumerator PreActionSetup()
    {
        yield return null;
    }

    protected override IEnumerator PostActionSetup()
    {
        yield return null;
    }

    public void ForceSuccess() => IsSuccess = true;
}