using UnityEngine;

[CreateAssetMenu(fileName = "NormalSegmentWidthModifierDefinition.asset", menuName = "Battle/Effects/Normal Segment Width Modifier Definition")]
public class NormalSegmentWidthModifierDefinition : EffectModifierDefinition
{    
    public override void Apply(BattleParticipant target, float value)
    {
        target.CharacterStats.ModifyNormalSegmentWidthModifier(value);
    }

    public override void Undo(BattleParticipant target, float value)
    {
        target.CharacterStats.ModifyNormalSegmentWidthModifier(-value);
    }

    public override string Stringify(float value)
    {
        var positiveSign = value > 0 ? "+" : "";
        return $"{positiveSign}{value * 100}%";
    }
}
