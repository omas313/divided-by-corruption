using UnityEngine;

[CreateAssetMenu(fileName = "CriticalSegmentWidthModifierDefinition.asset", menuName = "Battle/Effects/Critical Segment Width Modifier Definition")]
public class CriticalSegmentWidthModifierDefinition : EffectModifierDefinition
{    
    public override void Apply(BattleParticipant target, float value)
    {
        target.BattleModifiers.ModifyCriticalSegmentWidthModifier(value);
    }

    public override void Undo(BattleParticipant target, float value)
    {
        target.BattleModifiers.ModifyCriticalSegmentWidthModifier(-value);
    }

    public override string Stringify(float value)
    {
        var positiveSign = value > 0 ? "+" : "";
        return $"{positiveSign}{value * 100}%";
    }
}
