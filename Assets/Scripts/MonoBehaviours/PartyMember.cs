using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyMember : BattleParticipant
{
    public override string Name => _name;
    public override CharacterStats CharacterStats => _stats;

    [SerializeField] string _name;
    [SerializeField] CharacterStats _stats;

    SpriteRenderer _spriteRenderer;

    public override IEnumerator Die()
    {
        Debug.Log($"{Name} Died");
        BattleEvents.InvokePartyMemberDied(this);
        yield return null;
    }

    public override IEnumerator ReceiveAttack(BattleAttack attack)
    {
        // Debug.Log($"{Name} ReceiveAttack");
        _stats.ReduceCurrentHP(attack.Damage);
        yield return null;
    }

    public override IEnumerator PerformAttack(AttackDefinition attackDefinition, BattleParticipant receiver)
    {
        // do animations and other stuff
        yield return new WaitForSeconds(0.5f);
        
        var attack = new BattleAttack(attackDefinition);
        // add bonus from stats.damage later

        yield return receiver.ReceiveAttack(attack);

        Debug.Log($"{Name} {attack.Name} does {attack.Damage} damage to {receiver.Name}");
    }

    public override void SetRendererSortingOrder(int order)
    {
        _spriteRenderer.sortingOrder = order;
    }

    void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
}
