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

    public IEnumerator SpawnOnHitParticles(Vector3 position)
    {
        if (_onHitEffectsPrefab != null)
            yield return SpawnParticles(_onHitEffectsPrefab, position);
    }

    public IEnumerator SpawnCastParticles(Vector3 position)
    {
        if (_castEffectsPrefab != null)
            yield return SpawnParticles(_castEffectsPrefab, position);
    }

    public IEnumerator SpawnProjectile(Vector3 position, BattleParticipant target, bool isHit)
    {
        if (_projectilePrefab == null)
            yield break;
            
        var angle = Vector2.SignedAngle(Vector2.left, (target.BodyMidPointPosition - position).normalized);
        var rotation = Quaternion.Euler(0f, 0f, angle);
        var particles = Instantiate(_projectilePrefab, position, Quaternion.identity, GameObject.FindWithTag("Junk").transform)
            .GetComponent<ParticleSystem>();
        var particleSystemMain = particles.main;
        particleSystemMain.startRotation = new ParticleSystem.MinMaxCurve(Mathf.Deg2Rad * -angle);

        yield return new WaitForSeconds(2f);
    }

    IEnumerator SpawnParticles(GameObject prefab, Vector3 position)
    {
        var particles = Instantiate(prefab, position, Quaternion.identity, GameObject.FindWithTag("Junk").transform)
            .GetComponent<ParticleSystem>();
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !particles.isPlaying);
        yield return new WaitForSeconds(0.25f);
    }
}
