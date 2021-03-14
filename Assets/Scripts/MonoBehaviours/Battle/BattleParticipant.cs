using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleParticipant : MonoBehaviour
{
    public abstract string Name { get; }
    public abstract CharacterStats CharacterStats { get; }
    public bool IsDead => CharacterStats.CurrentHP <= 0;
    public Vector3 InitialPosition { get; protected set; }
    

    [SerializeField] protected AttackDefinition[] attacks;

    public abstract void Init(Vector3 position);
    public abstract IEnumerator Die();
    public abstract IEnumerator ReceiveAttack(BattleParticipant attacker, BattleAttack attack);
    public abstract void SetRendererSortingOrder(int order);

    protected IEnumerator CurrentAnimationFinished(Animator animator)
    {
        yield return new WaitForSeconds(0.15f);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
    }

    [ContextMenu("Kill")]
    public void CM_Kill()
    {
        CharacterStats.SetCurrentHP(0);
    }
}
