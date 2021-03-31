using System.Collections;
using UnityEngine;

[System.Serializable]
public class EffectModifier
{
    [SerializeField] EffectModifierDefinition _effectModifierDefinition;
    [SerializeField] float _value;

    public void Apply(BattleParticipant target)
    {
        _effectModifierDefinition.Apply(target, _value);
    }

    public void Undo(BattleParticipant target)
    {
        _effectModifierDefinition.Undo(target, _value);
    }
}
