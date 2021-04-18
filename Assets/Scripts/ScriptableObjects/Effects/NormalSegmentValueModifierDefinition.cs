using UnityEngine;

[CreateAssetMenu(fileName = "NormalSegmentValueModifierDefinition.asset", menuName = "Battle/Effects/Normal Segment Value Modifier Definition")]
public class NormalSegmentValueModifierDefinition : EffectModifierDefinition
{    
    public override void Apply(BattleParticipant target, float value)
    {
        target.CharacterStats.ModifyNormalSegmentValueModifier(value);
    }

    public override void Undo(BattleParticipant target, float value)
    {
        target.CharacterStats.ModifyNormalSegmentValueModifier(-value);
    }

    public override string Stringify(float value)
    {
        var positiveSign = value > 0 ? "+" : "";
        return $"{positiveSign}{value * 100}%";
    }
}
