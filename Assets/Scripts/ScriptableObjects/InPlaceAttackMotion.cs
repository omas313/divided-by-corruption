using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "InPlaceAttackMotion.asset", menuName = "In-Place Attack Motion")]
public class InPlaceAttackMotion : AttackMotionType
{
    public override IEnumerator PreAttackMotion(BattleParticipant attacker, BattleParticipant defender)
    {
        yield return null;
    }
    
    public override IEnumerator PostAttackMotion(BattleParticipant attacker, BattleParticipant defender)
    {
        yield return null;
    }
}
