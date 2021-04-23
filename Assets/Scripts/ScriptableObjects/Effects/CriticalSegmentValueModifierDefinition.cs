using UnityEngine;

[CreateAssetMenu(fileName = "CriticalSegmentValueModifierDefinition.asset", menuName = "Battle/Effects/Critical Segment Value Modifier Definition")]
public class CriticalSegmentValueModifierDefinition : EffectModifierDefinition
{    
    public override void Apply(BattleParticipant target, float value)
    {
        target.BattleModifiers.ModifyCriticalSegmentValueModifier(value);
    }

    public override void Undo(BattleParticipant target, float value)
    {
        target.BattleModifiers.ModifyCriticalSegmentValueModifier(-value);
    }

    public override string Stringify(float value)
    {
        var positiveSign = value > 0 ? "+" : "";
        return $"{positiveSign}{value * 100}%";
    }
}
