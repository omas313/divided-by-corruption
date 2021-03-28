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
    public ActionTargetterType AttackTargetType => _actionTargetterType;
    public AttackMotionType AttackMotionType => _attackMotionType;
    public GameObject OnHitEffectsPrefab => _onHitEffectsPrefab;
    public GameObject CastEffectsPrefab => _castEffectsPrefab;
    public GameObject ProjectilePrefab => _projectilePrefab;
    public GameObject EnvironmentalEffectPrefab => _environmentalEffectPrefab;
    public Color PowerColor => _powerColor;
    public float CastTime => _castTime;
    public bool HasTriggerAnimation => !string.IsNullOrWhiteSpace(_animationTriggerName);
    public bool HasProjectile => _projectilePrefab != null;
    public bool HasEnvironmentalEffect => _environmentalEffectPrefab != null;
    public bool HasOnHitParticles => _onHitEffectsPrefab != null;

    // todo need SpellAttackDefinition to hold spell-specific props

    [SerializeField] string _name;
    [SerializeField] string _animationTriggerName;
    [SerializeField] int _damage;
    [SerializeField] int _mpCost;
    [SerializeField] List<SegmentData> _segmentData;
    [SerializeField] ActionTargetterType _actionTargetterType;
    [SerializeField] AttackMotionType _attackMotionType;
    [SerializeField] GameObject _onHitEffectsPrefab;
    [SerializeField] GameObject _castEffectsPrefab;


    // todo: need seprate spell definition 
    [SerializeField] GameObject _projectilePrefab; 
    [SerializeField] GameObject _environmentalEffectPrefab;
    [SerializeField] Color _powerColor;
    [SerializeField] Color _secondaryPowerColor;
    [SerializeField] float _castTime;

    public IEnumerator PerformAction(BattleAction battleAction, List<PartyMember> party, List<Enemy> enemies)
    {
        var performer = battleAction.Performer;
        var attackDefinition = battleAction.AttackDefinition;

        battleAction.CalculateDamage();

        while (battleAction.HasAttacks)
        {
            if (attackDefinition.HasTriggerAnimation)
                yield return performer.TriggerAnimation(attackDefinition.AnimationTriggerName);

            yield return _actionTargetterType.Perform(battleAction, party, enemies);

            if (_actionTargetterType.ShouldStop())
                break;
        }
    }

    public void SpawnOnHitParticles(Vector3 position)
    {
        if (_onHitEffectsPrefab == null)
            return;

        Instantiate(_onHitEffectsPrefab, position, Quaternion.identity, GameObject.FindWithTag("Junk").transform);
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
