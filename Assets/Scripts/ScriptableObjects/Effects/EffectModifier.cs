using UnityEngine;

[CreateAssetMenu(fileName = "EffectModifier.asset", menuName = "Battle/Effects/Effect Modifier")]
public abstract class EffectModifier : ScriptableObject
{
    public string Name => _name;
    public string Description => _description;
    public Sprite IconSprite => _iconSprite;

    [SerializeField] string _name;
    [SerializeField] [TextArea(2, 5)] string _description;
    [SerializeField] Sprite _iconSprite;

    public abstract void Apply(BattleParticipant target);
    public abstract void Undo(BattleParticipant target);
}
