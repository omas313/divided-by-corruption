using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DashAttackMotion.asset", menuName = "Attack Motion/Dash Attack Motion")]
public class DashAttackMotion : AttackMotionType
{
    [SerializeField] string _forwardAnimationKey = "Dash";
    [SerializeField] string _backAnimationKey = "BackDash";

    public override IEnumerator PreAttackMotion(BattleParticipant attacker, BattleParticipant defender, AttackDefinition attackDefinition)
    {
        yield return MoveToPosition(attacker, defender.AttackReceiptPointPosition, attacker.CharacterStats.MotionSpeed, _forwardAnimationKey);
    }
    
    public override IEnumerator PostAttackMotion(BattleParticipant attacker, BattleParticipant defender, AttackDefinition attackDefinition)
    {
        yield return MoveToPosition(attacker, attacker.InitialPosition, attacker.CharacterStats.MotionSpeed, _backAnimationKey);
    }
}
