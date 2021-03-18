using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleParticipant : MonoBehaviour
{
    public abstract string Name { get; }
    public abstract CharacterStats CharacterStats { get; }
    public bool IsDead => CharacterStats.CurrentHP <= 0;
    public Sprite PortraitSprite => _portraitSprite;

    public Vector3 InitialPosition { get; protected set; }
    public Vector3 CurrentPosition => transform.position;
    public Vector3 BodyMidPointPosition => _bodyMidPoint.position;
    public Vector3 ProjectileCastPointPosition => _projectileCastPoint.position;
    public Vector3 AttackReceiptPointPosition => _attackReceiptPoint.position;

    [SerializeField] protected Transform _bodyMidPoint;
    [SerializeField] protected Transform _projectileCastPoint;
    [SerializeField] protected Transform _attackReceiptPoint;
    [SerializeField] Sprite _portraitSprite;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;

    public void InitPosition(Vector3 position)
    {
        InitialPosition = position;
        transform.position = position;
    }

    public virtual void SetRendererSortingOrder(int order)
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        spriteRenderer.sortingOrder = order;
    }

    public virtual IEnumerator PerformAction(BattleAction battleAction)
    {
        yield return Perform(battleAction);
    }

    public abstract IEnumerator Die();
    public abstract IEnumerator ReceiveAttack(BattleParticipant attacker, BattleAttack attack);

    protected IEnumerator Perform(BattleAction battleAction)
    {
        var initialPosition = transform.position;
        var target = battleAction.Target;
        var attackDefinition = battleAction.AttackDefinition;
        var attackMotionType = attackDefinition.AttackMotionType;
        var segmentResults = battleAction.AttackBarResult.SegmentsResults;

        yield return attackDefinition.SpawnCastParticles(transform.position);
        yield return attackMotionType.PreAttackMotion(this, target);

        CharacterStats.ReduceCurrentMP(attackDefinition.MPCost);

        for (var i = 0; i < segmentResults.Count ; i++)
        {
            var result = segmentResults[i];
            var attack = new BattleAttack(
                attackDefinition.Name, 
                Mathf.CeilToInt(attackDefinition.Damage * result.Multiplier),
                result.IsCritical);

            yield return PlayAnimation(attackDefinition.AnimationTriggerName);
            yield return attackDefinition.SpawnProjectile(ProjectileCastPointPosition, target, result.IsHit);

            if (result.IsHit)
            {
                StartCoroutine(attackDefinition.SpawnOnHitParticles(target.BodyMidPointPosition));
                yield return target.ReceiveAttack(this, attack);
            }

            SpawnResultHighlight(result, target);
            yield return new WaitForSeconds(0.25f);

            if (target.IsDead)
                break;
        }

        yield return attackMotionType.PostAttackMotion(this, target);
    }

    IEnumerator PlayAnimation(string name)
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        animator.SetTrigger(name);
        yield return new WaitForSeconds(0.15f);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
    }

    void SpawnResultHighlight(SegmentResult result, BattleParticipant target)
    {
        if (result.IsCritical)
            BattleEvents.InvokeAttackCritAt(target.CurrentPosition);
        else if (result.IsMiss)
            BattleEvents.InvokeAttackMissAt(target.CurrentPosition);
    }

    [ContextMenu("Kill")]
    public void CM_Kill()
    {
        CharacterStats.SetCurrentHP(0);
    }
}
