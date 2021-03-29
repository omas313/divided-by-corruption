using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionDefinition : ScriptableObject 
{
    public string Name => actionName;
    public string AnimationTriggerName => animationTriggerName;
    public ActionTargetterType ActionTargetterType => actionTargetterType;
    public ActionMotionType AttackMotionType => attackMotionType;
    public float DelayBeforeMotion => delayBeforeMotion;
    public GameObject EnergyParticlesPrefab => energyParticlesPrefab;
    public Color PrimaryPowerColor => primaryPowerColor;
    public Color SecondaryPowerColor => secondaryPowerColor;
    public bool HasEnergyParticles => energyParticlesPrefab != null;

    [SerializeField] protected string actionName;
    [SerializeField] protected string animationTriggerName;
    [SerializeField] protected ActionTargetterType actionTargetterType;
    [SerializeField] protected ActionMotionType attackMotionType;
    [SerializeField] protected float delayBeforeMotion = 1f;
    [SerializeField] protected GameObject energyParticlesPrefab;
    [SerializeField] protected Color primaryPowerColor;
    [SerializeField] protected Color secondaryPowerColor;

    public ParticleSystem SpawnEnergyParticles(Vector3 position, Transform parent)
    {
        var particles = Instantiate(energyParticlesPrefab, position, Quaternion.identity, parent).GetComponent<ParticleSystem>();
        SetParticlesGradient(particles);
        return particles;
    }
    
    void SetParticlesGradient(ParticleSystem particles)
    {
        var main = particles.main;
        
        if (secondaryPowerColor.a == 0)
            main.startColor = primaryPowerColor;
        else
        {
            var gradient = CreateGradient(primaryPowerColor, secondaryPowerColor);
            var minMaxGradient = new ParticleSystem.MinMaxGradient(gradient);
            minMaxGradient.mode = ParticleSystemGradientMode.RandomColor;
            main.startColor = minMaxGradient;
        }

        main.stopAction = ParticleSystemStopAction.Destroy;
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