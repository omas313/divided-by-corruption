using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : BattleAction, IActionBarAction, IAttackAction
{
    public ActionBarResult ActionBarResult { get; set; }
    public AttackDefinition AttackDefinition { get; set; }
    public List<SegmentData> SegmentData => AttackDefinition.SegmentData;
    public bool HasAttacks => _battleAttacks.Count > 0;

    public override bool IsValid => Performer != null
        && Target != null
        && ActionBarResult != null
        && AttackDefinition != null;

    Queue<BattleAttack> _battleAttacks;

    public AttackAction(BattleParticipant performer, BattleActionType battleActionType)
    {
        BattleActionType = battleActionType;
        Performer = performer;
    }

    public void InitBattleAttacks()
    {
        _battleAttacks = new Queue<BattleAttack>();

        foreach (var segmentResult in ActionBarResult.SegmentsResults)
        {
            var damage = Mathf.CeilToInt(AttackDefinition.Damage * segmentResult.Multiplier);
            _battleAttacks.Enqueue(new BattleAttack(AttackDefinition.Name, damage, segmentResult.IsHit, segmentResult.IsCritical));
        }
    }

    public BattleAttack GetNextBattleAttack() => _battleAttacks.Dequeue();

    public override IEnumerator Perform(List<PartyMember> party, List<Enemy> enemies)
    {
        yield return AttackDefinition.PreAttackMotion(Performer, Target); 
        Performer.ConsumeMP(AttackDefinition.MPCost);
        yield return AttackDefinition.PerformAttack(this, party, enemies);
        yield return AttackDefinition.PostAttackMotion(Performer, Target);
    }
}