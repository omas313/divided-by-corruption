using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : BattleParticipant
{    
    const string HIT_ANIMATION_BOOL_KEY = "IsGettingHit";
    const string DEATH_ANIMATION_BOOL_KEY = "IsDead";
    const string ATTACK_ANIMATION_TRIGGER_KEY = "Attack";
    const string IDLE_ANIMATION_TRIGGER_KEY = "Idle";
    const float DAMAGE_REDUCTION_FACTOR = 0.25f;

    public override string Name => _name;
    public override CharacterStats CharacterStats => _stats;
    public EnemyStats EnemyStats => _stats;
    public bool HasArmour => _stats.CurrentArmour > 0;

    [SerializeField] string _name;
    [SerializeField] EnemyStats _stats;

    SpriteRenderer _spriteRenderer;
    DamageType _lastDamageTypeReceived;
    Animator _animator;

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

    public IEnumerator PerformAttack(AttackDefinition attackDefinition, BattleParticipant receiver)
    {
        var attack = new BattleAttack(attackDefinition);
        // add bonus from stats.damage later

        _animator.SetTrigger(ATTACK_ANIMATION_TRIGGER_KEY);
        yield return CurrentAnimationFinished(_animator);

        yield return receiver.ReceiveAttack(this, attack);
    }
    

    public override IEnumerator ReceiveAttack(BattleParticipant attacker, BattleAttack attack)
    {
        _animator.SetBool(HIT_ANIMATION_BOOL_KEY, true);

        if (HasArmour)
        {
            _stats.ReduceCurrentArmour(attack.IsCritical ? 2 : 1);

            BattleEvents.InvokeEnemyArmourChanged(this, _stats.CurrentArmour, _stats.BaseArmour);
            BattleEvents.InvokeArmourDamageReceived(attacker, this, attack);
            
            if (!HasArmour)
                BattleEvents.InvokeArmourBreak(this);
        }
        else
        {
            _stats.ReduceCurrentHP(attack.Damage);
            BattleEvents.InvokeEnemyHealthChanged(this, _stats.CurrentHP, _stats.BaseHP);
            BattleEvents.InvokeHealthDamageReceived(attacker, this, attack);
        }

        yield return new WaitForSeconds(0.25f);
        _animator.SetBool(HIT_ANIMATION_BOOL_KEY, false);
    }

    public override void Init(Vector3 position)
    {
        InitialPosition = position;
        transform.position = position;
    }

    public override IEnumerator Die()
    {
        // Debug.Log($"{Name} died");
        BattleEvents.InvokeEnemyDied(this);
        _animator.SetBool(DEATH_ANIMATION_BOOL_KEY, true);
        yield return new WaitForSeconds(0.25f); 
    }

    public override void SetRendererSortingOrder(int order)
    {
        _spriteRenderer.sortingOrder = order;
    }

    bool ShouldReduceAttackDamage(DamageType damageType) => HasArmour && damageType == _lastDamageTypeReceived;

    void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        _animator.SetTrigger(IDLE_ANIMATION_TRIGGER_KEY);
        // Initialize(_definition);
    }
}

