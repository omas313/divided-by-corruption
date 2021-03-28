using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MultipleTargetActionPerformerType.asset", menuName = "ActionPerformerType/Multiple Target Action Performer Type")]
public class MultipleTargetActionPerformerType : ActionTargetterType
{
    List<Enemy> _enemies;

    public override IEnumerator Perform(AttackAction attackAction, List<PartyMember> party, List<Enemy> enemies)
    {
        _enemies = enemies;

        var attack = attackAction.GetNextBattleAttack();
        if (!attack.IsHit)
            yield break;

        var performer = attackAction.Performer;
        var attackDefinition = attackAction.AttackDefinition;

        yield return attackDefinition.SpawnEnvironmentalEffect();

        foreach (var enemy in enemies)
        {
            SpawnOnHitParticles(enemy, attackDefinition);
            performer.StartCoroutine(enemy.ReceiveAttack(performer, attack));
            InvokeResultEvents(attack, enemy);
        }

        yield return new WaitForSeconds(0.5f);
    }

    public override bool ShouldStop() => AreAllEnemiesDead();

    bool AreAllEnemiesDead()
    {
        foreach (var enemy in _enemies)
            if (!enemy.IsDead)
                return false;

        return true;
    }
}


