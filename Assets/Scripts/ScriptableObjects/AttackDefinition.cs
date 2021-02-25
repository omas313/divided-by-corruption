using UnityEngine;

[CreateAssetMenu(fileName = "AttackDefinition.asset", menuName = "Attack Definition")]
public class AttackDefinition : ScriptableObject
{
    public AttackType AttackType => _attackType;
    public DamageType DamageType => _damageType;
    public int Damage => _damage;
    public string Name => _name;
    public GameObject Prefab => _prefab;

    [SerializeField] AttackType _attackType;
    [SerializeField] DamageType _damageType;
    [SerializeField] int _damage;
    [SerializeField] string _name;
    [SerializeField] GameObject _prefab;
}
