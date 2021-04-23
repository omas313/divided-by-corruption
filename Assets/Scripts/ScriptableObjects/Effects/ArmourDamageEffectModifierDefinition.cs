using UnityEngine;

[CreateAssetMenu(fileName = "ArmourDamageEffectModifierDefinition.asset", menuName = "Battle/Effects/Armour Damage Effect Modifier Definition")]
public class ArmourDamageEffectModifierDefinition : EffectModifierDefinition
{    
    public override void Apply(BattleParticipant target, float value)
    {
        target.BattleModifiers.ModifyArmourDamageModifier(value);
    }

    public override void Undo(BattleParticipant target, float value)
    {
        target.BattleModifiers.ModifyArmourDamageModifier(-value);
    }

    public override string Stringify(float value)
    {
        var positiveSign = value > 0 ? "+" : "";
        return $"{positiveSign}{value * 100}%";
    }
}
