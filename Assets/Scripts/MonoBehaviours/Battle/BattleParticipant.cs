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

    public IEnumerator TriggerAnimation(string name)
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        animator.SetTrigger(name);
        yield return new WaitForSeconds(0.15f);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
    }

    public virtual void SetRendererSortingOrder(int order)
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        spriteRenderer.sortingOrder = order;
    }

    public virtual IEnumerator PerformAction(BattleAction battleAction, List<PartyMember> party, List<Enemy> enemies)
    {
        yield return Perform(battleAction, party, enemies);
    }

    public virtual void SetColliderActive(bool isActive)
    {
        var collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.Log("BattleParticipant doesn't have a collider");
            return;
        }

        collider.enabled = isActive;
    }

    public abstract IEnumerator Die();
    public abstract IEnumerator ReceiveAttack(BattleParticipant attacker, BattleAttack attack);

    protected IEnumerator Perform(BattleAction battleAction, List<PartyMember> party, List<Enemy> enemies)
    {
        var initialPosition = transform.position;
        var target = battleAction.Target;
        var segmentResults = battleAction.AttackBarResult.SegmentsResults;
        var attackDefinition = battleAction.AttackDefinition;

        var attackMotionType = attackDefinition.AttackMotionType;
        yield return attackMotionType.PreAttackMotion(this, target, attackDefinition); // attack def should do it

        CharacterStats.ReduceCurrentMP(attackDefinition.MPCost);
        yield return attackDefinition.PerformAction(battleAction, party, enemies);
        yield return attackMotionType.PostAttackMotion(this, target, attackDefinition); // attack def should do it
    }

    [ContextMenu("Kill")]
    public void CM_Kill()
    {
        CharacterStats.SetCurrentHP(0);
    }
}
