using UnityEngine;

[CreateAssetMenu(fileName = "HealthDamageEffectModifierDefinition.asset", menuName = "Battle/Effects/Health Damage Effect Modifier Definition")]
public class HealthDamageEffectModifierDefinition : EffectModifierDefinition
{    
    public override void Apply(BattleParticipant target, float value)
    {
        target.BattleModifiers.ModifyHealthDamageModifier(value);
    }

    public override void Undo(BattleParticipant target, float value)
    {
        target.BattleModifiers.ModifyHealthDamageModifier(-value);
    }

    public override string Stringify(float value)
    {
        var positiveSign = value > 0 ? "+" : "";
        return $"{positiveSign}{value * 100}%";
    }
}
