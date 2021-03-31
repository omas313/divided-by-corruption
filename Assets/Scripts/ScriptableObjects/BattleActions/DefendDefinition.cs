using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefendDefinition.asset", menuName = "Battle/Action Definition/Defend Definition")]
public class DefendDefinition : ActionDefinition
{
    public EffectDefinition DefendEffect => _defendEffect;
    public int Duration => _duration;

    [SerializeField] EffectDefinition _defendEffect;
    [SerializeField] int _duration = 1;
}