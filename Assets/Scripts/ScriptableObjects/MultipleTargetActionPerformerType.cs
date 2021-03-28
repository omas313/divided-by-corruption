using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MultipleTargetActionPerformerType.asset", menuName = "ActionPerformerType/Multiple Target Action Performer Type")]
public class MultipleTargetActionPerformerType : ActionPerformerType
{
    public override IEnumerator Perform(BattleAction battleAction, List<PartyMember> party, List<Enemy> enemies)
    {
        var performer = battleAction.Performer;
        var target = battleAction.Target;
        var segmentResults = battleAction.AttackBarResult.SegmentsResults;
        var attackDefinition = battleAction.AttackDefinition;

        for (var i = 0; i < segmentResults.Count ; i++)
        {
            var result = segmentResults[i];
            var attack = new BattleAttack(
                attackDefinition.Name, 
                Mathf.CeilToInt(attackDefinition.Damage * result.Multiplier),
                result.IsCritical);

            if (attackDefinition.HasEnvironmentalSpell && result.IsHit)
                yield return attackDefinition.SpawnEnvironmentalEffect();

            if (result.IsHit)
            {
                foreach (var enemy in enemies)
                {
                    SpawnOnHitParticles(performer, target, attackDefinition);
                    performer.StartCoroutine(enemy.ReceiveAttack(performer, attack));
                }
                yield return new WaitForSeconds(0.5f);
            }

            InvokeResultEvents(result, target);
            yield return new WaitForSeconds(0.25f);

            if (AreAllEnemiesDead(enemies))
                break;
        }
    }

    bool AreAllEnemiesDead(List<Enemy> enemies)
    {
        foreach (var enemy in enemies)
            if (!enemy.IsDead)
                return false;

        return true;
    }
}


