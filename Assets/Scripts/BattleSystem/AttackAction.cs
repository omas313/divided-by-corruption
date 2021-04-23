using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AttackAction : BattleAction, IAttackAction, IActionBarAction
{
    public override ActionDefinition ActionDefinition => AttackDefinition;
    public override bool IsValid => Performer != null
        && Targets != null
        && Targets.Count > 0
        && ActionBarResult != null
        && AttackDefinition != null;
    public override bool HasFailed { get; protected set; }

    public AttackDefinition AttackDefinition { get; set; }
    public bool HasAttacks => _battleAttacks.Count > 0;
    public ActionBarResult ActionBarResult { get; set; }
    public ActionBarData ActionBarData {get; set; }
    
    Queue<BattleAttack> _battleAttacks;

    public AttackAction(BattleParticipant performer, BattleActionType battleActionType, AttackDefinition attackDefinition = null)
    {
        BattleActionType = battleActionType;
        Performer = performer;
        ActionBarData = new ActionBarData()
        {
            NormalSegmentModifier = performer.BattleModifiers.NormalSegmentModifier,
            CriticalSegmentModifier = performer.BattleModifiers.CriticalSegmentModifier,
        };

        if (attackDefinition != null)  
            SetAttackDefinition(attackDefinition);
        
    }

    public void SetAttackDefinition(AttackDefinition attackDefinition)
    {
        AttackDefinition = attackDefinition;
        ActionBarData.SegmentsData = AttackDefinition.SegmentData;
    }

    protected override IEnumerator Perform(List<PartyMember> party, List<Enemy> enemies)
    {
        InitBattleAttacks();

        Performer.ConsumeMP(AttackDefinition.MPCost);

        BattleEvents.InvokeBattleParticipantsTargetted(Targets);

        if (AreAllAttacksMisses())
            HasFailed = true;

        while (HasAttacks)
        {
            var attack = GetNextBattleAttack();
            attack.Damage = Performer.BattleModifiers.ApplyOverallDamageModifier(attack.Damage);

            if (AttackDefinition.HasTriggerAnimation)
                yield return Performer.TriggerAnimation(AttackDefinition.AnimationTriggerName);

            if (attack.IsHit && AttackDefinition.HasEnvironmentalEffect)
                yield return AttackDefinition.SpawnEnvironmentalEffect();

            foreach (var target in Targets)
            {
                if (AttackDefinition.HasProjectile)
                    yield return AttackDefinition.SpawnProjectileEffect(Performer.ProjectileCastPointPosition, target, attack.IsHit);
                else
                    AttackDefinition.SpawnOnHitEffect(target.BodyMidPointPosition);

                Performer.StartCoroutine(target.ReceiveAttack(attack));
            }

            yield return new WaitForSeconds(0.25f);

            HandleSplashDamage(enemies, attack);

            if (AreTargetsDead())
                break;
        }
    }

    void InitBattleAttacks()
    {
        _battleAttacks = new Queue<BattleAttack>();

        foreach (var segmentResult in ActionBarResult.SegmentsResults)
        {
            var attack = CreateBattleAttack(segmentResult);
            _battleAttacks.Enqueue(attack);
        }
    }

    BattleAttack GetNextBattleAttack() => _battleAttacks.Dequeue();
    BattleAttack CreateBattleAttack(SegmentResult segmentResult) => new BattleAttack()
        {
            Attacker = Performer,
            Targets = Targets,
            Name = AttackDefinition.Name,
            Damage = Mathf.CeilToInt(AttackDefinition.Damage * segmentResult.Multiplier),
            IsHit = segmentResult.IsHit,
            IsCritical = segmentResult.IsCritical,
            AttackDefinition = AttackDefinition
        };

    void HandleSplashDamage(List<Enemy> enemies, BattleAttack attack)
    {
        if (Targets.Count == enemies.Count || !Performer.BattleModifiers.HasSplashDamage)
            return;

        var otherTargets = enemies.Where(e => !Targets.Contains(e)).ToList();
        foreach (var target in otherTargets)
        {
            var splashAttack = attack.CreateSplashAttackFor(target);
            Performer.StartCoroutine(target.ReceiveAttack(splashAttack));
        }
    }

    bool AreTargetsDead()
    {
        foreach (var target in Targets)
            if (!target.IsDead)
                return false;

        return true;
    }

    bool AreAllAttacksMisses() => _battleAttacks.All(ba => !ba.IsHit);
}
