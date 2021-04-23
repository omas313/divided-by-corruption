using UnityEngine;

[CreateAssetMenu(fileName = "AttackDamageEffectModifierDefinition.asset", menuName = "Battle/Effects/Attack Damage Effect Modifier Definition")]
public class AttackDamageEffectModifierDefinition : EffectModifierDefinition
{    
    public override void Apply(BattleParticipant target, float value)
    {
        target.BattleModifiers.ModifyOverallDamageModifier(value);
    }

    public override void Undo(BattleParticipant target, float value)
    {
        target.BattleModifiers.ModifyOverallDamageModifier(-value);
    }

    public override string Stringify(float value)
    {
        var positiveSign = value > 0 ? "+" : "";
        return $"{positiveSign}{value * 100}%";
    }
}
