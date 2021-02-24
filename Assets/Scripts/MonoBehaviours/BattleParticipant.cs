using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleParticipant : MonoBehaviour
{
    public abstract string Name { get; }
    public abstract CharacterStats CharacterStats { get; }
    
    public AttackDefinition[] Attacks => attacks;
    public AttackDefinition RandomAttack => attacks[UnityEngine.Random.Range(0, attacks.Length)];
    public bool IsDead => CharacterStats.CurrentHP <= 0;

    [SerializeField] protected AttackDefinition[] attacks;

    public abstract IEnumerator Die();
    public abstract IEnumerator PerformAttack(AttackDefinition attackDefinition, BattleParticipant receiver);
    public abstract IEnumerator ReceiveAttack(BattleAttack attack);
}
