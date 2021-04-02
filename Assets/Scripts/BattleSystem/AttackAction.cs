using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : BattleAction, IAttackAction, IActionBarAction
{
    public override ActionDefinition ActionDefinition => AttackDefinition;
    public override bool IsValid => Performer != null
        && Targets != null
        && Targets.Count > 0
        && ActionBarResult != null
        && AttackDefinition != null;

    public AttackDefinition AttackDefinition { get; set; }
    public bool HasAttacks => _battleAttacks.Count > 0;
    public ActionBarResult ActionBarResult { get; set; }
    public List<SegmentData> SegmentData => AttackDefinition.SegmentData;

    Queue<BattleAttack> _battleAttacks;

    public AttackAction(BattleParticipant performer, BattleActionType battleActionType)
    {
        BattleActionType = battleActionType;
        Performer = performer;
    }

    protected override IEnumerator Perform(List<PartyMember> party, List<Enemy> enemies)
    {
        InitBattleAttacks();
        
        Performer.ConsumeMP(AttackDefinition.MPCost);

        BattleEvents.InvokeBattleParticipantsTargetted(Targets);

        while (HasAttacks)
        {
            if (AttackDefinition.HasTriggerAnimation)
                yield return Performer.TriggerAnimation(AttackDefinition.AnimationTriggerName);

            var attack = GetNextBattleAttack();
            attack.Damage = Performer.CharacterStats.ApplyDamageModifier(attack.Damage);

            if (attack.IsHit && AttackDefinition.HasEnvironmentalEffect)
                yield return AttackDefinition.SpawnEnvironmentalEffect();

            foreach (var target in Targets)
            {
                if (AttackDefinition.HasProjectile)
                    yield return AttackDefinition.SpawnProjectileEffect(Performer.ProjectileCastPointPosition, target, attack.IsHit);
                else
                    AttackDefinition.SpawnOnHitEffect(target.BodyMidPointPosition);

                Performer.StartCoroutine(target.ReceiveAttack(Performer, attack));
                InvokeResultEvents(attack, target);
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
            var damage = Mathf.CeilToInt(AttackDefinition.Damage * segmentResult.Multiplier);
            _battleAttacks.Enqueue(new BattleAttack(AttackDefinition.Name, damage, segmentResult.IsHit, segmentResult.IsCritical));
        }
    }

    BattleAttack GetNextBattleAttack() => _battleAttacks.Dequeue();

    void InvokeResultEvents(BattleAttack attack, BattleParticipant target)
    {
        if (attack.IsCritical)
            BattleEvents.InvokeAttackCritAt(target.CurrentPosition);
        else if (!attack.IsHit)
            BattleEvents.InvokeAttackMissAt(target.CurrentPosition);
    }

    bool AreTargetsDead()
    {
        foreach (var target in Targets)
            if (!target.IsDead)
                return false;

        return true;
    }
}
