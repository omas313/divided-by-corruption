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
    public bool IsEnvironmentalSpell => _isEnvironmentalSpell;

    // todo need SpellAttackDefinition to hold spell-specific props

    [SerializeField] string _name;
    [SerializeField] string _animationTriggerName;
    [SerializeField] int _damage;
    [SerializeField] int _mpCost;
    [SerializeField] List<SegmentData> _segmentData;
    [SerializeField] AttackTargetType _attackTargetType;
    [SerializeField] AttackMotionType _attackMotionType;
    [SerializeField] GameObject _onHitEffectsPrefab;
    [SerializeField] GameObject _castEffectsPrefab;


    // todo: need seprate spell definition 
    [SerializeField] GameObject _projectilePrefab; // change to sfx prefab
    [SerializeField] Color _powerColor;
    [SerializeField] Color _secondaryPowerColor;
    [SerializeField] float _castTime;
    [SerializeField] bool _isEnvironmentalSpell;


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
        if (_secondaryPowerColor.a == 0)
            main.startColor = _powerColor;
        else
        {
            var gradient = CreateGradient(_powerColor, _secondaryPowerColor);
            var minMaxGradient = new ParticleSystem.MinMaxGradient(gradient);
            minMaxGradient.mode = ParticleSystemGradientMode.RandomColor;
            main.startColor = minMaxGradient;
        }
        
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
        if (_secondaryPowerColor.a == 0)
            main.startColor = _powerColor;
        else
        {
            var gradient = CreateGradient(_powerColor, _secondaryPowerColor);
            var minMaxGradient = new ParticleSystem.MinMaxGradient(gradient);
            minMaxGradient.mode = ParticleSystemGradientMode.RandomColor;
            main.startColor = minMaxGradient;
        }
        main.stopAction = ParticleSystemStopAction.Destroy;

        particles.Play();
        return particles;
    }

    public IEnumerator SpawnSpecialEffects(Vector3 castPosition, BattleParticipant target, bool isHit)
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

    public IEnumerator SpawnEnvironmentalSpell(bool isHit)
    {
        if (!isHit)
            yield break;

        var particles = Instantiate(_projectilePrefab).GetComponent<ParticleSystem>();
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

    Gradient CreateGradient(Color color1, Color color2)
    {
        var gradient = new Gradient();

        var colorkey = new GradientColorKey[2];
        colorkey[0].color = color1;
        colorkey[0].time = 0.45f;
        colorkey[1].color = color2;
        colorkey[1].time = 0.55f;

        var alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1f;
        alphaKey[0].time = 0f;
        alphaKey[1].alpha = 1f;
        alphaKey[1].time = 1f;

        gradient.SetKeys(colorkey, alphaKey);

        return gradient;
    }
}
