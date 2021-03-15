using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyMember : BattleParticipant
{
    const string CAST_ANIMATION_BOOL_KEY = "IsCastingSkill";
    const string HIT_ANIMATION_BOOL_KEY = "IsGettingHit";
    const string DEATH_ANIMATION_BOOL_KEY = "IsDead";
    const string LUNGE_ANIMATION_BOOL_KEY = "IsLunging";
    const string IDLE_ANIMATION_TRIGGER_KEY = "Idle";
    const string ATTACK_ANIMATION_TRIGGER_KEY = "Attack";
    
    public override string Name => _name;
    public override CharacterStats CharacterStats => _stats;
    public AttackDefinition NormalAttackDefinition => _normalAttackDefinition;
    public List<AttackDefinition> SpecialAttacksDefinitions => _specialAttackDefinitions;

    [SerializeField] Transform _castPoint;
    [SerializeField] string _name;
    [SerializeField] CharacterStats _stats;
    [SerializeField] ParticleSystem _castParticles;

    [SerializeField] AttackDefinition _normalAttackDefinition;
    [SerializeField] List<AttackDefinition> _specialAttackDefinitions;

    SpriteRenderer _spriteRenderer;
    Animator _animator;


    public IEnumerator PerformAction(BattleAction battleAction)
    {
        if (battleAction.BattleActionType == BattleActionType.Attack)
            yield return PerformNormalAttack(battleAction.AttackBarResult, battleAction.Target);
        // else if (selectedActionType == ActionType.Skill)
        //     yield return PerformSkillAttack(attackMultipliers, receiver);
    }

    public IEnumerator PerformNormalAttack(AttackBarResult attackBarResult, BattleParticipant receiver)
    {
        var initialPosition = transform.position;

        yield return _normalAttackDefinition.AttackMotionType.PreAttackMotion(this, receiver);

        for (var i = 0; i < attackBarResult.SegmentsResults.Count ; i++)
        {
            var result = attackBarResult.SegmentsResults[i];
            var attack = new BattleAttack(
                _normalAttackDefinition.Name, 
                Mathf.CeilToInt(_normalAttackDefinition.Damage * result.Multiplier),
                result.IsCritical);

            _animator.SetTrigger(ATTACK_ANIMATION_TRIGGER_KEY);
            yield return CurrentAnimationFinished(_animator);

            if (!result.IsMiss)
                yield return receiver.ReceiveAttack(this, attack);

            if (result.IsCritical)
                BattleEvents.InvokeAttackCrit(receiver.transform.position);
            else if (result.IsMiss)
                BattleEvents.InvokeAttackMiss(receiver.transform.position);

            yield return new WaitForSeconds(0.25f);
        }

        yield return _normalAttackDefinition.AttackMotionType.PostAttackMotion(this, receiver);
    }

    IEnumerator PerformSkillAttack(AttackDefinition attackDefinition, BattleParticipant receiver)
    {
        _animator.SetBool(CAST_ANIMATION_BOOL_KEY, true);
        var main = _castParticles.main;
        // main.startColor = attackDefinition.PowerColor;
        _castParticles.Play();


        var angle = Vector2.SignedAngle(Vector2.left, (receiver.transform.position + new Vector3(0f, 1f, 0f) - _castPoint.position).normalized);
        var rotation = Quaternion.Euler(0f, 0f, angle);
        var particles = Instantiate(attackDefinition.OnHitEffectsPrefab, _castPoint.position, rotation);
        var particleSystemMain = particles.GetComponent<ParticleSystem>().main;
        particleSystemMain.startRotation = new ParticleSystem.MinMaxCurve(Mathf.Deg2Rad * -angle);

        yield return new WaitForSeconds(2f);

        _castParticles.Stop();
        _animator.SetBool(CAST_ANIMATION_BOOL_KEY, false);
    }

    public override void Init(Vector3 position)
    {
        InitialPosition = position;
        transform.position = position;
    }

    public override IEnumerator Die()
    {
        Debug.Log($"{Name} Died");
        _animator.SetBool(DEATH_ANIMATION_BOOL_KEY, true);
        BattleEvents.InvokePartyMemberDied(this);
        yield return null;
    }

    public override IEnumerator ReceiveAttack(BattleParticipant attacker, BattleAttack attack)
    {
        // Debug.Log($"{Name} ReceiveAttack");
        _animator.SetBool(HIT_ANIMATION_BOOL_KEY, true);
        _stats.ReduceCurrentHP(attack.Damage);
        BattleEvents.InvokeHealthDamageReceived(attacker, this, attack);

        yield return new WaitForSeconds(0.5f);
        _animator.SetBool(HIT_ANIMATION_BOOL_KEY, false);
    }

    public override void SetRendererSortingOrder(int order)
    {
        _spriteRenderer.sortingOrder = order;
    }

    void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }
}
