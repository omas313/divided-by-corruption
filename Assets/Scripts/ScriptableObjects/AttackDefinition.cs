using UnityEngine;

[CreateAssetMenu(fileName = "AttackDefinition.asset", menuName = "Attack Definition")]
public class AttackDefinition : ScriptableObject
{
    public AttackType AttackType => _attackType;
    public int Damage => _damage;
    public string Name => _name;

    [SerializeField] AttackType _attackType;
    [SerializeField] int _damage;
    [SerializeField] string _name;
}
