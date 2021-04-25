using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDefinition.asset", menuName = "Enemy Definition")]
public class EnemyDefinition : ScriptableObject
{
    public GameObject GameObjectprefab => _gameObjectprefab;
    public EnemyStats Stats => _stats;
    public Sprite Sprite => _sprite;
    public string Name => _name;
    public AttackDefinition[] Attacks => _attacks;
    
    [SerializeField] GameObject _gameObjectprefab;
    [SerializeField] EnemyStats _stats;
    [SerializeField] AttackDefinition[] _attacks;
    [SerializeField] Sprite _sprite;
    [SerializeField] string _name;
}
