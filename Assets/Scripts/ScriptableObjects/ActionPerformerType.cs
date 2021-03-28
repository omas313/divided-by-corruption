using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionPerformerType : ScriptableObject
{
    public abstract IEnumerator Perform(BattleAction battleAction, List<PartyMember> party, List<Enemy> enemies);

    protected void SpawnOnHitParticles(BattleParticipant performer, BattleParticipant target, AttackDefinition attackDefinition)
    {
        performer.StartCoroutine(attackDefinition.SpawnOnHitParticles(target.BodyMidPointPosition));
    }

    protected void InvokeResultEvents(SegmentResult result, BattleParticipant target)
    {
        if (result.IsCritical)
            BattleEvents.InvokeAttackCritAt(target.CurrentPosition);
        else if (result.IsMiss)
            BattleEvents.InvokeAttackMissAt(target.CurrentPosition);
    }
}
