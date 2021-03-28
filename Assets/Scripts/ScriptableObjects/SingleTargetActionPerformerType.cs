using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SingleTargetActionPerformerType.asset", menuName = "ActionPerformerType/Single Target Action Performer Type")]
public class SingleTargetActionPerformerType : ActionPerformerType
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

            if (attackDefinition.HasTriggerAnimation)
                yield return performer.TriggerAnimation(attackDefinition.AnimationTriggerName);

            if (attackDefinition.HasProjectile)
                yield return attackDefinition.SpawnProjectileEffect(performer.ProjectileCastPointPosition, target, result.IsHit);

            if (result.IsHit)
            {
                SpawnOnHitParticles(performer, target, attackDefinition);
                yield return target.ReceiveAttack(performer, attack);
            }

            InvokeResultEvents(result, target);
            yield return new WaitForSeconds(0.25f);

            if (target.IsDead)
                break;
        }
    }
}