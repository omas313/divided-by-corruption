using UnityEngine;

[CreateAssetMenu(fileName = "DefenseEffectModifierDefinition.asset", menuName = "Battle/Effects/Defense Effect Modifier Definition")]
public class DefenseEffectModifierDefinition : EffectModifierDefinition
{
    public override void Apply(BattleParticipant target, float value)
    {
        target.BattleModifiers.ModifyDefenseModifier(value);
    }

    public override void Undo(BattleParticipant target, float value)
    {
        target.BattleModifiers.ModifyDefenseModifier(-value);
    }
 
    public override string Stringify(float value)
    {
        var positiveSign = value > 0 ? "+" : "";
        return $"{positiveSign}{value * 100}%";
    }
}