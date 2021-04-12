using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackDefinition.asset", menuName = "Battle/Action Definition/Attack Definition")]
public class AttackDefinition : ActionDefinition
{
    public int Damage => _damage;
    public float SplashDamageModifier => _splashDamageModifier;
    public int MPCost => _mpCost;
    public List<SegmentData> SegmentData => _segmentData;
    public GameObject OnHitEffectsPrefab => _onHitEffectsPrefab;
    public GameObject ProjectilePrefab => _projectilePrefab;
    public GameObject EnvironmentalEffectPrefab => _environmentalEffectPrefab;
    public bool HasTriggerAnimation => !string.IsNullOrWhiteSpace(animationTriggerName);
    public bool HasProjectile => _projectilePrefab != null;
    public bool HasEnvironmentalEffect => _environmentalEffectPrefab != null;
    public bool HasOnHitParticles => _onHitEffectsPrefab != null;
    public List<EffectDefinition> ComboEffectDefinitions => _comboEffectDefinitions;

    [SerializeField] int _damage;
    [SerializeField] float _splashDamageModifier = 0.5f;
    [SerializeField] int _mpCost;
    [SerializeField] List<SegmentData> _segmentData;
    [SerializeField] GameObject _onHitEffectsPrefab;

    // todo: need seprate spell definition maybe ????
    [SerializeField] GameObject _projectilePrefab; 
    [SerializeField] GameObject _environmentalEffectPrefab;

    [SerializeField] List<EffectDefinition> _comboEffectDefinitions;

    public void SpawnOnHitEffect(Vector3 position)
    {
        if (!HasOnHitParticles)
            return;

        Instantiate(_onHitEffectsPrefab, position, Quaternion.identity, GameObject.FindWithTag("Junk").transform);
    }

    public IEnumerator SpawnProjectileEffect(Vector3 castPosition, BattleParticipant target, bool isHit)
    {
        if (!HasProjectile)
            yield break;

        var angle = Vector2.SignedAngle(Vector2.right, (target.BodyMidPointPosition - castPosition).normalized);
        var rotation = Quaternion.Euler(0f, 0f, angle);

        var particles = Instantiate(_projectilePrefab, castPosition, rotation, GameObject.FindWithTag("Junk").transform)
            .GetComponent<ParticleSystem>();
        var particleSystemMain = particles.main;
        particleSystemMain.startRotation = new ParticleSystem.MinMaxCurve(Mathf.Deg2Rad * -angle);


        if (isHit)
            yield return HandleParticleCollision(particles, target);
        else
            yield return new WaitForSeconds(particleSystemMain.startLifetime.constant);
    }

    public IEnumerator SpawnEnvironmentalEffect()
    {
        if (!HasEnvironmentalEffect)
            yield break;

        var particles = Instantiate(_environmentalEffectPrefab).GetComponent<ParticleSystem>();
        particles.transform.parent = GameObject.FindWithTag("Junk").transform;

        yield return new WaitUntil(() => !particles.isPlaying && particles.particleCount == 0);
    }

    IEnumerator HandleParticleCollision(ParticleSystem particles, BattleParticipant target)
    {
        var collision = particles.collision;
        collision.enabled = true;        

        var collided = false;
        var collisionHandler = particles.GetComponent<ParticleCollisionHandler>();
        if (collisionHandler == null)
        {
            Debug.Log("No collision handler on particle");
            yield break;
        }
        collisionHandler.Collided += () => collided = true;

        target.SetColliderActive(true);
        yield return new WaitUntil(() => collided);
        target.SetColliderActive(false);
    }
}