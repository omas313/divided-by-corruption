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
    const string IDLE_ANIMATION_TRIGGER_KEY = "Idle";
    const string ATTACK_ANIMATION_TRIGGER_KEY = "Attack";
    const float LUNGE_SPEED = 4f;
    readonly Vector3 _attackPositioningOffset = new Vector3(1f, 0f, 0f);

    
    public override string Name => _name;
    public override CharacterStats CharacterStats => _stats;

    [SerializeField] Transform _castPoint;
    [SerializeField] string _name;
    [SerializeField] CharacterStats _stats;

    SpriteRenderer _spriteRenderer;
    Animator _animator;
    private Vector3 _initialPosition;

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

    public override IEnumerator PerformAttack(AttackDefinition attackDefinition, BattleParticipant receiver)
    {
        // do animations and other stuff
        yield return new WaitForSeconds(0.5f);
        
        var attack = new BattleAttack(attackDefinition);
        // add bonus from stats.damage later

        // can mvoe to SO
        if (attackDefinition.IsLunge)
        {
            _initialPosition = transform.position;
            yield return FadeOut();
            yield return MoveToPosition(receiver.transform.position - _attackPositioningOffset);
            yield return FadeIn();
        }

        // can mvoe to SO
        if (attackDefinition.IsSpell)
            yield return CastSpell(attackDefinition.EffectsPrefab, receiver);
        else
        {
            _animator.SetTrigger(ATTACK_ANIMATION_TRIGGER_KEY);
            yield return CurrentAnimationFinished(_animator);
        }

        yield return receiver.ReceiveAttack(this, attack);

        if (attackDefinition.IsLunge)
        {
            yield return FadeOut();
            yield return MoveToPosition(_initialPosition);
            yield return FadeIn();
        }

         if (attackDefinition.IsSpell)
            _animator.SetBool(CAST_ANIMATION_BOOL_KEY, false);
    }

    IEnumerator CastSpell(GameObject attackPrefab, BattleParticipant receiver)
    {
        _animator.SetBool(CAST_ANIMATION_BOOL_KEY, true);

        var angle = Vector2.SignedAngle(Vector2.left, (receiver.transform.position - _castPoint.position).normalized);
        var rotation = Quaternion.Euler(0f, 0f, angle);
        var particles = Instantiate(attackPrefab, _castPoint.position, rotation);
        var particleSystemMain = particles.GetComponent<ParticleSystem>().main;
        particleSystemMain.startRotation = new ParticleSystem.MinMaxCurve(Mathf.Deg2Rad * -angle);

        yield return new WaitForSeconds(2f);

        _animator.SetBool(CAST_ANIMATION_BOOL_KEY, false);
    }

    public override void SetRendererSortingOrder(int order)
    {
        _spriteRenderer.sortingOrder = order;
    }

    IEnumerator FadeOut(float speed = 1f)
    {
        var startTime = Time.time;
        var currentColor = _spriteRenderer.color;

        while (currentColor.a > 0f)
        {
            float alphaAmount = (Time.time - startTime) * speed;
            currentColor.a = Mathf.Lerp(currentColor.a, 0f, alphaAmount);
            _spriteRenderer.color = currentColor;
            yield return null;
        }
    }

    IEnumerator FadeIn(float speed = 1f)
    {
        var startTime = Time.time;
        var currentColor = _spriteRenderer.color;

        while (currentColor.a < 1f)
        {
            float alphaAmount = (Time.time - startTime) * speed;
            currentColor.a = Mathf.Lerp(currentColor.a, 1f, alphaAmount);
            _spriteRenderer.color = currentColor;
            yield return null;
        }
    }

    IEnumerator MoveToPosition(Vector3 destination)
    {
        var startTime = Time.time;
        var totalDistance = Vector2.Distance(transform.position, destination);

        while (Vector2.Distance(destination, transform.position) > 0.1f)
        {
            float distanceToCover = (Time.time - startTime) * LUNGE_SPEED;
            float distancePercentage = distanceToCover / totalDistance;
            transform.position = Vector3.Lerp(transform.position, destination, distancePercentage);
            yield return null;
        }

    }

    void SetVisibility(bool isVisible) => _spriteRenderer.color = isVisible ? Color.white : Color.clear;

    void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }
}
