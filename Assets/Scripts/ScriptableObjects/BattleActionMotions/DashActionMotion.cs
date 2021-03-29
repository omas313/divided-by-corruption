using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DashActionMotion.asset", menuName = "Battle/Action Motion/Dash Action Motion")]
public class DashActionMotion : ActionMotionType
{
    [SerializeField] string _forwardAnimationKey = "Dash";
    [SerializeField] string _backAnimationKey = "BackDash";

    public override IEnumerator PreActionMotion(BattleParticipant performer, BattleParticipant target)
    {
        yield return MoveToPosition(performer, target.AttackReceiptPointPosition, performer.CharacterStats.MotionSpeed, _forwardAnimationKey);
    }
    
    public override IEnumerator PostActionMotion(BattleParticipant performer, BattleParticipant target)
    {
        yield return MoveToPosition(performer, performer.InitialPosition, performer.CharacterStats.MotionSpeed, _backAnimationKey);
    }

    IEnumerator MoveToPosition(BattleParticipant performer, Vector3 destination, float speed, string animationKey)
    {
        var startTime = Time.time;
        var totalDistance = Vector2.Distance(performer.transform.position, destination);
        var animator = performer.GetComponent<Animator>();
        
        animator.SetBool(animationKey, true);

        while (Vector2.Distance(destination, performer.transform.position) > 0.1f)
        {
            float distanceToCover = (Time.time - startTime) * speed;
            float distancePercentage = distanceToCover / totalDistance;
            performer.transform.position = Vector3.Lerp(performer.transform.position, destination, distancePercentage);
            yield return null;
        }

        animator.SetBool(animationKey, false);
    }
}
