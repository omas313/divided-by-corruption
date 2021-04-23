using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleParticipant : MonoBehaviour
{
    public abstract string Name { get; }
    public abstract CharacterStats CharacterStats { get; }
    public abstract BattleModifiers BattleModifiers { get; } 
    public bool IsDead => CharacterStats.CurrentHP <= 0;
    public Sprite PortraitSprite => portraitSprite;

    public Transform Transform => transform;
    public Vector3 InitialPosition { get; protected set; }
    public Vector3 CurrentPosition => transform.position;
    public Vector3 BodyMidPointPosition => bodyMidPoint.position;
    public Vector3 ProjectileCastPointPosition => projectileCastPoint.position;
    public Vector3 AttackReceiptPointPosition => attackReceiptPoint.position;
    public Transform TopMarkerTransform => topMarkerPoint;

    public EffectsManager EffectsManager => effectsManager;

    [SerializeField] protected Transform bodyMidPoint;
    [SerializeField] protected Transform projectileCastPoint;
    [SerializeField] protected Transform attackReceiptPoint;
    [SerializeField] protected Transform topMarkerPoint;
    [SerializeField] protected Sprite portraitSprite;

    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected EffectsManager effectsManager;

    public void InitPosition(Vector3 position)
    {
        InitialPosition = position;
        transform.position = position;
    }

    public int TakeMP(int amount) => CharacterStats.TakeMP(amount);
    public void ConsumeMP(int amount) => CharacterStats.ReduceCurrentMP(amount);
    public void AddMP(int amount) => CharacterStats.IncreaseCurrentMP(amount);

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

    public virtual void EndTurn()
    {
        effectsManager.ReduceEffectDurations();
    }

    public abstract IEnumerator Die();
    public abstract IEnumerator ReceiveAttack(BattleAttack attack);

    [ContextMenu("Kill")]
    public void CM_Kill()
    {
        CharacterStats.SetCurrentHP(0);
    }

    protected virtual void Awake()
    {
        effectsManager = new EffectsManager(this);
        animator = GetComponent<Animator>();
    }
}
