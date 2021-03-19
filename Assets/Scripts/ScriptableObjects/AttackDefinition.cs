using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackDefinition.asset", menuName = "Attack Definition")]
public class AttackDefinition : ScriptableObject
{
    public string Name => _name;
    public string AnimationTriggerName => _animationTriggerName;
    public int Damage => _damage;
    public int MPCost => _mpCost;
    public int SegmentsCount => _segmentData.Count;
    public List<SegmentData> SegmentData => _segmentData;
    public AttackTargetType AttackTargetType => _attackTargetType;
    public AttackMotionType AttackMotionType => _attackMotionType;
    public GameObject OnHitEffectsPrefab => _onHitEffectsPrefab;
    public GameObject CastEffectsPrefab => _castEffectsPrefab;
    public GameObject ProjectilePrefab => _projectilePrefab;
    public Color PowerColor => _powerColor;
    public float CastTime => _castTime;

    [SerializeField] string _name;
    [SerializeField] string _animationTriggerName;
    [SerializeField] int _damage;
    [SerializeField] int _mpCost;
    [SerializeField] List<SegmentData> _segmentData;
    [SerializeField] AttackTargetType _attackTargetType;
    [SerializeField] AttackMotionType _attackMotionType;
    [SerializeField] GameObject _onHitEffectsPrefab;
    [SerializeField] GameObject _castEffectsPrefab;
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] Color _powerColor;
    [SerializeField] float _castTime;

    public IEnumerator SpawnOnHitParticles(Vector3 position)
    {
        if (_onHitEffectsPrefab == null)
            yield break;

        var particles = Instantiate(_onHitEffectsPrefab, position, Quaternion.identity, GameObject.FindWithTag("Junk").transform)
            .GetComponent<ParticleSystem>();
        var main = particles.main;
        main.startColor = _powerColor;
        main.loop = false;
        main.stopAction = ParticleSystemStopAction.Destroy;

        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !particles.isPlaying);
        yield return new WaitForSeconds(0.25f);
    }

    public IEnumerator SpawnAndStopCastParticles(Vector3 position)
    {
        if (_castEffectsPrefab == null)
            yield break;

        var particles = Instantiate(_castEffectsPrefab, position, Quaternion.identity, GameObject.FindWithTag("Junk").transform)
            .GetComponent<ParticleSystem>();
        var main = particles.main;
        main.startColor = _powerColor;
        main.stopAction = ParticleSystemStopAction.Destroy;
        particles.Play();

        yield return new WaitForSeconds(_castTime);
        particles.Stop();
    }

    public ParticleSystem SpawnCastParticles(Vector3 position)
    {
        if (_castEffectsPrefab == null)
            return null;

        var particles = Instantiate(_castEffectsPrefab, position, Quaternion.identity, GameObject.FindWithTag("Junk").transform)
            .GetComponent<ParticleSystem>();
        var main = particles.main;
        main.startColor = _powerColor;
        main.stopAction = ParticleSystemStopAction.Destroy;
        particles.Play();

        return particles;
    }

    public IEnumerator SpawnProjectile(Vector3 castPosition, BattleParticipant target, bool isHit)
    {
        if (_projectilePrefab == null)
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
