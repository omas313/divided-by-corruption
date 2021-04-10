using System;
using System.Collections.Generic;
using System.Linq;

public class Effect
{
    public bool HasFinished { get; private set; }
    public int Duration => _duration;
    public List<EffectModifier> Modifiers => _modifiers;
    public string ShortDescription => String.Join($"\n", Modifiers.Select(m => $"{m.ShortDescription} +{m.ValueString}"));

    List<EffectModifier> _modifiers;
    int _duration;

    public Effect(EffectDefinition effectDefinition)
    {
        _modifiers = effectDefinition.Modifiers;
        _duration = effectDefinition.Duration;
    }

    public void SetDuration(int duration) => _duration = duration;

    public void ApplyEffect(BattleParticipant target)
    {
        foreach (var modifier in _modifiers)
            modifier.Apply(target);
    }

    public void UndoEffect(BattleParticipant target)
    {
        HasFinished = true;
        foreach (var modifier in _modifiers)
            modifier.Undo(target);
    }

    public void ReduceDuration(BattleParticipant target)
    {
        _duration = Math.Max(0, _duration - 1);

        if (_duration <= 0)
            UndoEffect(target);
    }
}
