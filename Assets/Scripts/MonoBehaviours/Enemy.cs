using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : BattleParticipant
{
    const float DAMAGE_REDUCTION_FACTOR = 0.25f;

    public event Action<int, int> HealthChanged;
    public event Action<int, int> ArmourChanged;

    public override string Name => _name;
    public override CharacterStats CharacterStats => _stats;

    [SerializeField] string _name;
    [SerializeField] EnemyStats _stats;

    SpriteRenderer _spriteRenderer;
    DamageType _lastDamageTypeReceived;

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

    public override IEnumerator PerformAttack(AttackDefinition attackDefinition, BattleParticipant receiver)
    {
        // do animations and other stuff
        yield return new WaitForSeconds(0.5f);
        
        var attack = new BattleAttack(attackDefinition);
        // add bonus from stats.damage later

        yield return receiver.ReceiveAttack(attack);

        Debug.Log($"{Name} {attack.Name} does {attack.Damage} damage to {receiver.Name}");
    }
    
    public bool HasArmour => _stats.CurrentArmour >= 0;
    public bool ShouldReduceAttackDamage(DamageType damageType) => HasArmour && damageType == _lastDamageTypeReceived;

    public override IEnumerator ReceiveAttack(BattleAttack attack)
    {
        var damageToInflict = attack.Damage;

        if (ShouldReduceAttackDamage(attack.DamageType))
            damageToInflict = (int)(attack.Damage * DAMAGE_REDUCTION_FACTOR);

        if (HasArmour)
        {
            damageToInflict -= _stats.CurrentArmour;
            _stats.SetCurrentArmour(damageToInflict >= 0 ? 0 : -damageToInflict);

            BattleEvents.InvokeEnemyArmourChanged(this, _stats.CurrentArmour, _stats.BaseArmour);

            _lastDamageTypeReceived = attack.DamageType;
        }
        
        if (damageToInflict > 0)
        {
            _stats.ReduceCurrentHP(damageToInflict);
            BattleEvents.InvokeEnemyHealthChanged(this, _stats.CurrentHP, _stats.BaseHP);
        }

        yield return new WaitForSeconds(0.5f);
    }

    public override IEnumerator Die()
    {
        Debug.Log($"{Name} died");
        BattleEvents.InvokeEnemyDied(this);
        yield return new WaitForSeconds(0.25f); 
        // do animatiosn
    }

    public override void SetRendererSortingOrder(int order)
    {
        _spriteRenderer.sortingOrder = order;
    }

    void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        // Initialize(_definition);
    }
}

