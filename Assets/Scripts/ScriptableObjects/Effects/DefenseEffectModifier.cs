using UnityEngine;

public class DefenseEffectModifier : EffectModifier
{    
    public float Percentage => _percentage;

    [SerializeField] bool _reversed;    
    [SerializeField] [Range(0f, 1f)] float _percentage;

    public override void Apply(BattleParticipant target)
    {
        if (_reversed)
            Undo(target);
        else
            target.CharacterStats.IncreaseDefenseModifier(_percentage);
    }

    public override void Undo(BattleParticipant target)
    {
        if (_reversed)
            Apply(target);
        else
            target.CharacterStats.DecreaseDefenseModifier(_percentage);
    }
}