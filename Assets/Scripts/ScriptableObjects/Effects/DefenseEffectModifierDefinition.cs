using UnityEngine;

[CreateAssetMenu(fileName = "DefenseEffectModifierDefinition.asset", menuName = "Battle/Effects/Defense Effect Modifier Definition")]
public class DefenseEffectModifierDefinition : EffectModifierDefinition
{
    public override void Apply(BattleParticipant target, float value)
    {
        target.CharacterStats.IncreaseDefenseModifier(value);
    }

    public override void Undo(BattleParticipant target, float value)
    {
        target.CharacterStats.DecreaseDefenseModifier(value);
    }
 
    public override string Stringify(float value) => $"{value * 100}%";
}