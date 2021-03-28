using System.Collections.Generic;
using UnityEngine;

public class BattleAction
{
    public BattleActionType BattleActionType { get; set; }
    public BattleParticipant Performer { get; set; }
    public BattleParticipant Target { get; set; }
    public AttackBarResult AttackBarResult { get; set; }
    public AttackDefinition AttackDefinition { get; set; }
    public Queue<BattleAttack> BattleAttacks { get; private set; }
    public bool HasAttacks => BattleAttacks.Count > 0;

    public bool IsValid => BattleActionType != BattleActionType.None
        && Performer != null
        && Target != null
        && AttackBarResult != null
        && AttackDefinition != null;

    public void CalculateDamage()
    {
        BattleAttacks = new Queue<BattleAttack>();

        foreach (var segmentResult in AttackBarResult.SegmentsResults)
        {
            var damage = Mathf.CeilToInt(AttackDefinition.Damage * segmentResult.Multiplier);
            BattleAttacks.Enqueue(new BattleAttack(AttackDefinition.Name, damage, segmentResult.IsHit, segmentResult.IsCritical));
        }
    }

    public BattleAttack GetNextBattleAttack() => BattleAttacks.Dequeue();
}
