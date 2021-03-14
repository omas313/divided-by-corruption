using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DashAttackMotion.asset", menuName = "Dash Attack Motion")]
public class DashAttackMotion : AttackMotionType
{
    [SerializeField] protected Vector3 attackPositioningOffset = new Vector3(1f, 0f, 0f);
    [SerializeField] string _forwardAnimationKey = "Dash";
    [SerializeField] string _backAnimationKey = "BackDash";

    public override IEnumerator PreAttackMotion(BattleParticipant attacker, BattleParticipant defender)
    {
        yield return MoveToPosition(attacker, defender.transform.position - attackPositioningOffset, attacker.CharacterStats.MotionSpeed, _forwardAnimationKey);
    }
    
    public override IEnumerator PostAttackMotion(BattleParticipant attacker, BattleParticipant defender)
    {
        yield return MoveToPosition(attacker, attacker.InitialPosition, attacker.CharacterStats.MotionSpeed, _backAnimationKey);
    }
}
