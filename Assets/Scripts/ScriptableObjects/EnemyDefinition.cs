using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDefinition.asset", menuName = "Battle/Enemy Definition")]
public class EnemyDefinition : ScriptableObject
{
    public string Name => _name;
    public GameObject GameObjectprefab => _gameObjectprefab;
    public EnemyStats Stats => _stats;
    public AttackDefinition[] Attacks => _attacks;
    public EffectDefinition[] EffectDefinitions => _effectDefinitions;
    
    [SerializeField] string _name;
    [SerializeField] GameObject _gameObjectprefab;
    [SerializeField] [Tooltip("Starting Stats.")] EnemyStats _stats;
    [SerializeField] AttackDefinition[] _attacks;
    [SerializeField] [Tooltip("Effects that can be learnt by player.")] EffectDefinition[] _effectDefinitions;
}
