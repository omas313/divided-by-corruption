using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "InPlaceAttackMotion.asset", menuName = "In-Place Attack Motion")]
public class InPlaceAttackMotion : AttackMotionType
{
    const string IN_PLACE_CAST_ANIMATION_KEY = "IsCasting";

    public override IEnumerator PreAttackMotion(BattleParticipant attacker, BattleParticipant defender)
    {
        var animator = attacker.GetComponent<Animator>();

        animator.SetBool(IN_PLACE_CAST_ANIMATION_KEY, true);
        yield return new WaitForSeconds(0.1f);
        animator.SetBool(IN_PLACE_CAST_ANIMATION_KEY, false);
    }
    
    public override IEnumerator PostAttackMotion(BattleParticipant attacker, BattleParticipant defender)
    {
        yield return null;
    }
}
