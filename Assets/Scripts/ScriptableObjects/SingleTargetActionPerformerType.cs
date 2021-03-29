// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// [CreateAssetMenu(fileName = "SingleTargetActionPerformerType.asset", menuName = "ActionPerformerType/Single Target Action Performer Type")]
// public class SingleTargetActionPerformerType : ActionTargetterType
// {
//     BattleParticipant _target;

//     public override IEnumerator Perform(AttackAction attackAction, List<PartyMember> party, List<Enemy> enemies)
//     {
//         _target = attackAction.Targets; 
//         // "GetTargets"

//         var performer = attackAction.Performer;
//         var attackDefinition = attackAction.AttackDefinition;
//         var attack = attackAction.GetNextBattleAttack();

//         // spawn env is it exists & isHit

//         // loop over targets
//             // spawn projectile particle if it exists

//         yield return attackDefinition.SpawnProjectileEffect(performer.ProjectileCastPointPosition, _target, attack.IsHit);

//         if (attack.IsHit)
//         {
//             SpawnOnHitParticles(_target, attackDefinition);
//             yield return _target.ReceiveAttack(performer, attack);
//         }

//         InvokeResultEvents(attack, _target);
//         yield return new WaitForSeconds(0.25f);

//     }

//     public override bool ShouldStop() => _target.IsDead;
// }