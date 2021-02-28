using UnityEngine;

[CreateAssetMenu(fileName = "AttackDefinition.asset", menuName = "Attack Definition")]
public class AttackDefinition : ScriptableObject
{
    public AttackType AttackType => _attackType;
    public DamageType DamageType => _damageType;
    public int Damage => _damage;
    public string Name => _name;
    public GameObject EffectsPrefab => _effectsPrefab;
    public bool IsLunge => _isLunge;
    public bool IsSpell => _isSpell;
    public Color PowerColor => _powerColor;

    [SerializeField] AttackType _attackType;
    [SerializeField] DamageType _damageType;
    [SerializeField] int _damage;
    [SerializeField] string _name;
    [SerializeField] GameObject _effectsPrefab;
    [SerializeField] bool _isLunge;
    [SerializeField] bool _isSpell;
    [SerializeField] Color _powerColor;
}
