using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "JumpActionMotion.asset", menuName = "Battle/Action Motion/Jump Action Motion")]
public class JumpActionMotion : ActionMotionType
{
    [SerializeField] protected Vector3 attackPositioningOffset = new Vector3(1f, 0f, 0f);
    [SerializeField] string _forwardAnimationKey = "Jump";
    [SerializeField] string _backAnimationKey = "BackDash";
    [SerializeField] float _height = 10f;
    [SerializeField] float _midPointDelay = 1f;

    public override IEnumerator PreActionMotion(BattleParticipant performer, BattleParticipant target)
    {
        var midPointX = (target.transform.position.x + performer.transform.position.x) * 0.5f;
        var destination = new Vector3(midPointX, _height);
        yield return MoveToPosition(performer, destination, performer.CharacterStats.MotionSpeed, _forwardAnimationKey);

        yield return new WaitForSeconds(_midPointDelay);

        yield return MoveToPosition(performer, target.transform.position - attackPositioningOffset, performer.CharacterStats.MotionSpeed, _forwardAnimationKey);
    }
    
    public override IEnumerator PostActionMotion(BattleParticipant perfromer, BattleParticipant target)
    {
        yield return MoveToPosition(perfromer, perfromer.InitialPosition, perfromer.CharacterStats.MotionSpeed, _backAnimationKey);
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
