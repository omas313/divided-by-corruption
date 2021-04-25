using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManualTesting : MonoBehaviour
{
    [SerializeField] EffectDefinition[] _effectDefinitions;
    [SerializeField] PartyMember _partyMember;

    [ContextMenu("Apply Effect")]
    public void ApplyEffect()
    {
        foreach (var effectDefinition in _effectDefinitions)
        {
            var effect = new Effect(effectDefinition);
            _partyMember.EffectsManager.AddEffect(effect);
        }
    }
}
