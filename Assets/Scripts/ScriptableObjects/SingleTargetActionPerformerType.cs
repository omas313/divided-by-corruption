using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SingleTargetActionPerformerType.asset", menuName = "ActionPerformerType/Single Target Action Performer Type")]
public class SingleTargetActionPerformerType : ActionTargetterType
{
    BattleParticipant _target;

    public override IEnumerator Perform(BattleAction battleAction, List<PartyMember> party, List<Enemy> enemies)
    {
        _target = battleAction.Target;

        var performer = battleAction.Performer;
        var attackDefinition = battleAction.AttackDefinition;
        var attack = battleAction.GetNextBattleAttack();

        yield return attackDefinition.SpawnProjectileEffect(performer.ProjectileCastPointPosition, _target, attack.IsHit);

        if (attack.IsHit)
        {
            SpawnOnHitParticles(performer, _target, attackDefinition);
            yield return _target.ReceiveAttack(performer, attack);
        }

        InvokeResultEvents(attack, _target);
        yield return new WaitForSeconds(0.25f);

    }

    public override bool ShouldStop() => _target.IsDead;
}