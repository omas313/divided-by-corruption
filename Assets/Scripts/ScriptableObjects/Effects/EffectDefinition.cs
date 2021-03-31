using System;
using System.Collections.Generic;
using UnityEngine;

// this should be SO so we can give to designer to play around with creating effects with submodifiers
// maybe we can hav another SO that chooses what to affect eg. Damage, Defence, AttackBar

// goals: 
//      we want to slot effects onto skills i.e. effects needs to be SO
//      in the effect SO, 
//              we have a display name and maybe icon later
//              we want to choose what is changing i.e. the "EffectModifier"
//                  Eg. Increase Damage | Increase Attack segments on Bar | add fire damage   <------ should these be enums or also SO's so we can do the logic there
//              we also want to place a number to decide how it should change
//              we need a percentage to determine chance of application

[CreateAssetMenu(fileName = "EffectDefinition.asset", menuName = "Battle/Effects/Effect Definition")]
public class EffectDefinition : ScriptableObject
{
    public bool HasFinished { get; private set; }
    public int Duration => _duration;
    public List<EffectModifier> Modifiers => _modifiers;

    [SerializeField] [Tooltip("# Turns")] int _duration;
    [SerializeField] List<EffectModifier> _modifiers;

    BattleParticipant _target;

    public void ApplyEffect(BattleParticipant target)
    {
        _target = target;

        foreach (var modifier in Modifiers)
            modifier.Apply(_target);
    }

    public void UndoEffect()
    {
        HasFinished = true;
        foreach (var modifier in Modifiers)
            modifier.Undo(_target);
    }

    public void ReduceDuration()
    {
        _duration = Math.Max(0, Duration - 1);

        if (_duration == 0)
            UndoEffect();
    }
}
