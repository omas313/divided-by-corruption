using System.Collections;
using UnityEngine;

public abstract class AttackMotionType : ScriptableObject
{
    public abstract IEnumerator PreAttackMotion(BattleParticipant attacker, BattleParticipant defender, AttackDefinition attackDefinition);
    public abstract IEnumerator PostAttackMotion(BattleParticipant attacker, BattleParticipant defender, AttackDefinition attackDefinition);

    protected IEnumerator MoveToPosition(BattleParticipant attacker, Vector3 destination, float speed, string animationKey)
    {
        var startTime = Time.time;
        var totalDistance = Vector2.Distance(attacker.transform.position, destination);
        var animator = attacker.GetComponent<Animator>();
        
        animator.SetBool(animationKey, true);

        while (Vector2.Distance(destination, attacker.transform.position) > 0.1f)
        {
            float distanceToCover = (Time.time - startTime) * speed;
            float distancePercentage = distanceToCover / totalDistance;
            attacker.transform.position = Vector3.Lerp(attacker.transform.position, destination, distancePercentage);
            yield return null;
        }

        animator.SetBool(animationKey, false);
    }
}
