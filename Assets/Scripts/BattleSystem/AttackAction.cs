using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    public List<SegmentData> SegmentData => AttackDefinition.SegmentData;

    Queue<BattleAttack> _battleAttacks;

    public AttackAction(BattleParticipant performer, BattleActionType battleActionType, AttackDefinition attackDefinition = null)
    {
        BattleActionType = battleActionType;
        Performer = performer;
        AttackDefinition = attackDefinition;
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
            attack.Damage = Performer.CharacterStats.ApplyDamageModifier(attack.Damage);

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

            if (AreTargetsDead())
                break;
        }

        yield return new WaitForSeconds(0.5f);
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

    bool AreTargetsDead()
    {
        foreach (var target in Targets)
            if (!target.IsDead)
                return false;

        return true;
    }

    bool AreAllAttacksMisses() => _battleAttacks.All(ba => !ba.IsHit);
}
