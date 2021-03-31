using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectDefinition.asset", menuName = "Battle/Effects/Effect Definition")]
public class EffectDefinition : ScriptableObject
{
    public int Duration => _duration;
    public List<EffectModifier> Modifiers => _modifiers;

    [SerializeField] [Tooltip("# Turns")] int _duration;
    [SerializeField] List<EffectModifier> _modifiers;
}