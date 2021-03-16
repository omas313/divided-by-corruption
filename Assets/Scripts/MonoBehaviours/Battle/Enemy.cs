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
    [SerializeField] float _criticalChance = 0.2f; // put in enemy stats or possibly let enemy also have randomized Attack bar results
    [SerializeField] float _additionalCriticalMultiplier = 1f; // put in enemy stats or possibly let enemy also have randomized Attack bar results
    [SerializeField] float _missChance = 0.2f; // put in enemy stats or attack definitnion

    [SerializeField] AttackDefinition[] _attackDefinitions;


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

    public override IEnumerator PerformAction(BattleAction battleAction)
    {
        battleAction.Attacker = this;
        battleAction.BattleActionType = BattleActionType.Attack;
        battleAction.AttackDefinition = _attackDefinitions[UnityEngine.Random.Range(0, _attackDefinitions.Length)];

        var segmentResults = new List<SegmentResult>();
        foreach (var segment in battleAction.AttackDefinition.SegmentData)
        {
            var isMiss = UnityEngine.Random.value < _missChance;
            var isCritical = isMiss ? false : UnityEngine.Random.value < _criticalChance;
            var multiplier = isMiss ? 0f : (isCritical ? segment.CriticalMultiplier : segment.NormalMultiplier);
            var result = new SegmentResult(segment, multiplier, isCritical, isMiss);
            segmentResults.Add(result);
        }

        battleAction.AttackBarResult = new AttackBarResult(segmentResults);

        yield return base.PerformAction(battleAction);
    }

    public override IEnumerator ReceiveAttack(BattleParticipant attacker, BattleAttack attack)
    {
        animator.SetBool(HIT_ANIMATION_BOOL_KEY, true);

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
        animator.SetBool(HIT_ANIMATION_BOOL_KEY, false);
    }

    public override IEnumerator Die()
    {
        // Debug.Log($"{Name} died");
        BattleEvents.InvokeEnemyDied(this);
        animator.SetBool(DEATH_ANIMATION_BOOL_KEY, true);
        yield return new WaitForSeconds(0.25f); 
    }

    void Awake()
    {
        animator = GetComponent<Animator>();

        animator.SetTrigger(IDLE_ANIMATION_TRIGGER_KEY);
        // Initialize(_definition);
    }
}

