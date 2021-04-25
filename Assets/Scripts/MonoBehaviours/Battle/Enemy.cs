using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : BattleParticipant
{    
    const string HIT_ANIMATION_TRIGGER_KEY = "IsGettingHit";
    const string DEATH_ANIMATION_BOOL_KEY = "IsDead";
    const string ATTACK_ANIMATION_TRIGGER_KEY = "Attack";
    const string IDLE_ANIMATION_TRIGGER_KEY = "Idle";
    const float DAMAGE_REDUCTION_FACTOR = 0.25f;

    public override string Name => _name;
    public override CharacterStats CharacterStats => _stats;
    public EnemyStats EnemyStats => _stats;
    public override BattleModifiers BattleModifiers => _battleModifiers;
    public bool HasArmour => _stats.CurrentArmour > 0;

    [SerializeField] string _name;
    [SerializeField] EnemyStats _stats;
    [SerializeField] BattleModifiers _battleModifiers;
    [SerializeField] ParticleSystem _armourParticles;
    [SerializeField] SpriteRenderer _armouredSpriteRenderer;
    [SerializeField] SpriteRenderer _armourlessSpriteRenderer;
    [SerializeField] RuntimeAnimatorController _armourlessAnimatorController;

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

    public BattleAction GetNextAction(List<PartyMember> party, List<Enemy> enemies)
    {
        // todo: setup ActionDefinitions
        // todo: decide next course of action based on rules (definitions???)

        var attackAction = new AttackAction(this, BattleActionType.Attack);
        var randomTarget = party[UnityEngine.Random.Range(0, party.Count)];
        attackAction.Targets.Add(randomTarget);
        attackAction.AttackDefinition = _attackDefinitions[UnityEngine.Random.Range(0, _attackDefinitions.Length)];

        BattleEvents.InvokeEnemySelectedTargets(attackAction.Targets);

        var segmentResults = new List<SegmentResult>();
        foreach (var segment in attackAction.AttackDefinition.SegmentData)
        {
            var isMiss = UnityEngine.Random.value < _stats.MissChance;
            var isCritical = isMiss ? false : UnityEngine.Random.value < _stats.CriticalChance;
            
            float multiplier;
            if (isMiss)
                multiplier = 0f;
            else if (isCritical)
                multiplier = segment.CriticalMultiplier + _stats.AdditionalCriticalMultiplier;
            else 
                multiplier = segment.NormalMultiplier;

            var result = new SegmentResult(segment, multiplier, isCritical, isMiss);
            segmentResults.Add(result);
        }

        attackAction.ActionBarResult = new ActionBarResult(segmentResults);
        return attackAction;
    }

    public override IEnumerator ReceiveAttack(BattleAttack attack)
    {
        if (!attack.IsHit)
        {
            BattleEvents.InvokeMissedAttackReceived(this);
            yield break;
        }

        animator.SetTrigger(HIT_ANIMATION_TRIGGER_KEY);

        var damageLeftToTake = BattleModifiers.ApplyDefenseModifier(attack.Damage);
        var armourDamageTaken = 0;

        if (HasArmour)
            (armourDamageTaken, damageLeftToTake) = TakePossibleArmourDamage(damageLeftToTake, attack.Attacker.BattleModifiers);
        
        attack.ActualDamageTaken += armourDamageTaken;
        
        if (damageLeftToTake > 0)
        {
            damageLeftToTake = attack.Attacker.BattleModifiers.ApplyHealthDamageModifier(damageLeftToTake);
            attack.ActualDamageTaken += damageLeftToTake;
            CharacterStats.ReduceCurrentHP(damageLeftToTake);
            BattleEvents.InvokeEnemyHealthChanged(this, _stats.CurrentHP, _stats.BaseHP);
        }

        InvokeReceivedAttackEvents(attack, armourDamageTaken, damageLeftToTake);
        
        yield return new WaitForSeconds(0.25f);
    }

    public override IEnumerator Die()
    {
        BattleEvents.InvokeEnemyDied(this);
        animator.SetBool(DEATH_ANIMATION_BOOL_KEY, true);
        foreach (var particles in GetComponentsInChildren<ParticleSystem>())
            particles.Stop();
        yield return new WaitForSeconds(0.25f); 
    }

    public override void SetRendererSortingOrder(int order)
    {
        _armouredSpriteRenderer.sortingOrder = order;
        _armourlessSpriteRenderer.sortingOrder = order;
    }

    (int, int) TakePossibleArmourDamage(int damage, BattleModifiers battleModifiers)
    {
        var initialArmour = _stats.CurrentArmour;
        var armourDamage = battleModifiers.ApplyArmourDamageModifier(damage);
        _stats.ReduceCurrentArmour(armourDamage);

        BattleEvents.InvokeEnemyArmourChanged(this, _stats.CurrentArmour, _stats.BaseArmour);
        
        if (!HasArmour)
            RemoveArmour();

        return (HasArmour ? armourDamage : initialArmour, HasArmour ? 0 : damage - initialArmour);
    }

    void InvokeReceivedAttackEvents(BattleAttack attack, int armourDamageTaken, int healthDamageTaken)
    {
        if (!attack.IsSplashAttack)
            BattleEvents.InvokeBattleAttackReceived(this, attack);

        if (attack.IsHit && attack.Damage == 0)
        {
            if (HasArmour)
                BattleEvents.InvokeArmourDamageReceived(this, 0, false);
            else
                BattleEvents.InvokeHealthDamageReceived(this, 0, false);

            return;
        }
        
        if (armourDamageTaken > 0)
            BattleEvents.InvokeArmourDamageReceived(this, armourDamageTaken, attack.IsCritical && healthDamageTaken == 0);

        if (healthDamageTaken > 0)
            BattleEvents.InvokeHealthDamageReceived(this, healthDamageTaken, attack.IsCritical);
    }

    void RemoveArmour()
    {
        BattleEvents.InvokeArmourBreak(this);

        _battleModifiers.ModifyDefenseModifier(-_stats.ArmourDefenseModifierPercentage);

        _armourParticles.Play();
        _armouredSpriteRenderer.enabled = false;
        _armourlessSpriteRenderer.enabled = true;
        spriteRenderer = _armourlessSpriteRenderer;

        animator.runtimeAnimatorController = _armourlessAnimatorController;
    }

    void Initialize()
    {
        _battleModifiers.ModifyDefenseModifier(_stats.ArmourDefenseModifierPercentage);
    }

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = _armouredSpriteRenderer;

        Initialize();
        // Initialize(_definition);
    }

    [ContextMenu("dlg sprite")]
    public void DLG()
    {
        Debug.Log(spriteRenderer.sprite.name);
    }
}

