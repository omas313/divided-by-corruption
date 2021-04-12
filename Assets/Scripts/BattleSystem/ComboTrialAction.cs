using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ComboTrialAction : BattleAction
{
    public override ActionDefinition ActionDefinition => ComboTrialDefinition;
    public override bool IsValid => Performer != null && HasUIResult;
    public override bool HasFailed { get; protected set; }

    public ComboTrialDefinition ComboTrialDefinition { get; set; }
    public bool HasUIResult { get; private set; }
    public AttackDefinition AttackDefinition { get; private set; }

    Combo _combo;

    public ComboTrialAction(PartyMember starter, AttackDefinition attackDefinition, Combo combo)
    {
        Performer = starter;
        AttackDefinition = attackDefinition;
        _combo = combo;
    }

    public void SetResult(bool success)
    {
        HasUIResult = true;
        HasFailed = !success;
    }

    protected override IEnumerator Perform(List<PartyMember> party, List<Enemy> enemies)
    {
        if (!HasUIResult)
        {
            Debug.Log("Error @ ComboTrialAction: performing combo trial action without UI result");
            yield break;
        }

        if (HasFailed)
        {
            _combo.Break();
            BattleEvents.InvokeComboBroken(Performer as PartyMember);
            yield break;
        }

        _combo.Start();

        var comboFollower = _combo.GetParticipantAfter(Performer as PartyMember);
        var effectsString = new StringBuilder();

        foreach (var existingComboEffect in _combo.Effects)
        {
            comboFollower.EffectsManager.AddEffect(existingComboEffect);
            effectsString.AppendLine(existingComboEffect.ShortDescription);
        }

        foreach (var effectDefinition in AttackDefinition.ComboEffectDefinitions)
        {
            var effect = new Effect(effectDefinition);
            _combo.AddEffect(effect);
            comboFollower.EffectsManager.AddEffect(effect);
            effectsString.AppendLine(effect.ShortDescription);
        }

        BattleEvents.InvokeComboEffectsGained(comboFollower, effectsString.ToString());

        var targetsToAdd = Targets
            .Select(t => t as Enemy)
            .Where(e => !e.IsDead)
            .ToList();
        _combo.AddTargets(targetsToAdd);
    }

    protected override IEnumerator PreActionSetup()
    {
        yield return null;
    }

    protected override IEnumerator PostActionSetup()
    {
        yield return null;
    }
}