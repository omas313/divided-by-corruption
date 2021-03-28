using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionTargetterType : ScriptableObject
{
    public abstract IEnumerator Perform(BattleAction battleAction, List<PartyMember> party, List<Enemy> enemies);

    public abstract bool ShouldStop();


    protected void SpawnOnHitParticles(BattleParticipant target, AttackDefinition attackDefinition)
    {
        if (attackDefinition.HasOnHitParticles)
            attackDefinition.SpawnOnHitParticles(target.BodyMidPointPosition);
    }

    protected void InvokeResultEvents(BattleAttack attack, BattleParticipant target)
    {
        if (attack.IsCritical)
            BattleEvents.InvokeAttackCritAt(target.CurrentPosition);
        else if (!attack.IsHit)
            BattleEvents.InvokeAttackMissAt(target.CurrentPosition);
    }
}
