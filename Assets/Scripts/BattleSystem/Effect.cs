using System;
using System.Collections.Generic;

public class Effect
{
    public bool HasFinished { get; private set; }
    public int Duration => _duration;
    public List<EffectModifier> Modifiers => _modifiers;

    List<EffectModifier> _modifiers;
    int _duration;
    BattleParticipant _target;

    public Effect(EffectDefinition effectDefinition, BattleParticipant target)
    {
        _modifiers = effectDefinition.Modifiers;
        _duration = effectDefinition.Duration;
        _target = target;
    }

    public void SetDuration(int duration) => _duration = duration;

    public void ApplyEffect()
    {
        foreach (var modifier in _modifiers)
            modifier.Apply(_target);
    }

    public void UndoEffect()
    {
        HasFinished = true;
        foreach (var modifier in _modifiers)
            modifier.Undo(_target);
    }

    public void ReduceDuration()
    {
        _duration = Math.Max(0, _duration - 1);

        if (_duration <= 0)
            UndoEffect();
    }
}
