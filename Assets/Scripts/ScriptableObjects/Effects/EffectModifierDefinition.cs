using UnityEngine;

[System.Serializable]
public abstract class EffectModifierDefinition : ScriptableObject
{
    public string Name => _name;
    public string Description => $"{_description} {_value}";
    public string ShortDescription => _shortDescription;
    public Sprite IconSprite => _iconSprite;

    [SerializeField] string _name;
    [SerializeField] [TextArea(2, 5)] string _description;
    [SerializeField] string _shortDescription;
    [SerializeField] Sprite _iconSprite;

    float _value;

    public abstract void Apply(BattleParticipant target, float value);
    public abstract void Undo(BattleParticipant target, float value);
    public abstract string Stringify(float value);
}
