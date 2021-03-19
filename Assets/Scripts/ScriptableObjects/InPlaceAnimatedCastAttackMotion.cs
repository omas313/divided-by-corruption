using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "InPlaceAnimatedCastAttackMotion.asset", menuName = "Attack Motion/In-Place Animated Cast Attack Motion")]
public class InPlaceAnimatedCastAttackMotion : AttackMotionType
{
    const string IN_PLACE_CAST_ANIMATION_KEY = "IsCasting";

    ParticleSystem _castParticles;

    public override IEnumerator PreAttackMotion(BattleParticipant attacker, BattleParticipant defender, AttackDefinition attackDefinition)
    {
        attacker.GetComponent<Animator>().SetBool(IN_PLACE_CAST_ANIMATION_KEY, true);

        _castParticles = attackDefinition.SpawnCastParticles(attacker.transform.position);
        yield return new WaitForSeconds(attackDefinition.CastTime);
    }
    
    public override IEnumerator PostAttackMotion(BattleParticipant attacker, BattleParticipant defender, AttackDefinition attackDefinition)
    {
        attacker.GetComponent<Animator>().SetBool(IN_PLACE_CAST_ANIMATION_KEY, false);

        if (_castParticles != null)
        {
            _castParticles.Stop();
            Destroy(_castParticles.gameObject);
            _castParticles = null;
        }

        yield return null;
    }
}
