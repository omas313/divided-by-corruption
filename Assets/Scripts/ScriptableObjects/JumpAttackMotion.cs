using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "JumpAttackMotion.asset", menuName = "Attack Motion/Jump Attack Motion")]
public class JumpAttackMotion : AttackMotionType
{
    [SerializeField] protected Vector3 attackPositioningOffset = new Vector3(1f, 0f, 0f);
    [SerializeField] string _forwardAnimationKey = "Jump";
    [SerializeField] string _backAnimationKey = "BackDash";
    [SerializeField] float _height = 10f;
    [SerializeField] float _midPointDelay = 1f;

    public override IEnumerator PreAttackMotion(BattleParticipant attacker, BattleParticipant defender, AttackDefinition attackDefinition)
    {
        var midPointX = (defender.transform.position.x + attacker.transform.position.x) * 0.5f;
        var destination = new Vector3(midPointX, _height);
        yield return MoveToPosition(attacker, destination, attacker.CharacterStats.MotionSpeed, _forwardAnimationKey);

        yield return new WaitForSeconds(_midPointDelay);

        yield return MoveToPosition(attacker, defender.transform.position - attackPositioningOffset, attacker.CharacterStats.MotionSpeed, _forwardAnimationKey);
    }
    
    public override IEnumerator PostAttackMotion(BattleParticipant attacker, BattleParticipant defender, AttackDefinition attackDefinition)
    {
        yield return MoveToPosition(attacker, attacker.InitialPosition, attacker.CharacterStats.MotionSpeed, _backAnimationKey);
    }
}
