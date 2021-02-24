using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : BattleParticipant
{
    public override string Name => _name;
    public override CharacterStats CharacterStats => _stats;

    [SerializeField] string _name;
    [SerializeField] CharacterStats _stats;
    // [SerializeField] EnemyDefinition _definition;

    // public void Initialize(EnemyDefinition definition)
    // {
    //     _definition = definition;

    //     _stats = new CharacterStats();
    //     _stats.SetCurrentHP(_definition.Stats.BaseHP);
    //     _stats.SetCurrentSpeed(_definition.Stats.BaseSpeed);
    //     _stats.SetCurrentMP(_definition.Stats.BaseMP);

    //     GetComponentInChildren<SpriteRenderer>().sprite = _definition.Sprite;
    //     _name = _definition.name;
    //     attacks = _definition.Attacks;
    // }

    PartyMember GetTarget(List<PartyMember> playerParty) => playerParty[UnityEngine.Random.Range(0, playerParty.Count)];

    IEnumerator PerformAttack(BattleParticipant attackReceiver)
    {
        var randomAttack = attacks[UnityEngine.Random.Range(0, attacks.Length)];
        
        // do animations and other stuff
        yield return attackReceiver.ReceiveAttack(new BattleAttack(randomAttack.Damage));
        Debug.Log($"{Name} {randomAttack.Name} does {randomAttack.Damage} damage to {attackReceiver.Name}");
    }
    
    public override IEnumerator Die()
    {
        Debug.Log($"{Name} died");
        yield return new WaitForSeconds(0.25f); 
        // do animatiosn
    }

    void Awake()
    {
        // Initialize(_definition);
    }

    public override IEnumerator SetCommand(List<PartyMember> playerParty, List<Enemy> enemies)
    {
        Debug.Log($"{Name} SetCommand");
        yield return null;
    }

    public override IEnumerator ReceiveAttack(BattleAttack attack)
    {
        Debug.Log($"{Name} ReceiveAttack");
        yield return null;
    }
}

