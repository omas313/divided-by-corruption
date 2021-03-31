using UnityEngine;

[CreateAssetMenu(fileName = "AttackDamageEffectModifierDefinition.asset", menuName = "Battle/Effects/Attack Damage Effect Modifier Definition")]
public class AttackDamageEffectModifierDefinition : EffectModifierDefinition
{    
    public override void Apply(BattleParticipant target, float value)
    {
        target.CharacterStats.IncreaseDamageModifier(value);
    }

    public override void Undo(BattleParticipant target, float value)
    {
        target.CharacterStats.DecreaseDamageModifier(value);
    }
}
