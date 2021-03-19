using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "InPlaceCastAttackMotion.asset", menuName = "Attack Motion/In-Place Cast Attack Motion")]
public class InPlaceCastAttackMotion : AttackMotionType
{
    public override IEnumerator PreAttackMotion(BattleParticipant attacker, BattleParticipant defender, AttackDefinition attackDefinition)
    {
        yield return attackDefinition.SpawnAndStopCastParticles(attacker.transform.position);
    }
    
    public override IEnumerator PostAttackMotion(BattleParticipant attacker, BattleParticipant defender, AttackDefinition attackDefinition)
    {
        yield return null;
    }
}
