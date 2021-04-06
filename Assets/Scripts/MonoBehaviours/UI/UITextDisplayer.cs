using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UITextDisplayer : MonoBehaviour
{
    [SerializeField] UIFloatingText _damageTextPrefab;
    [SerializeField] float _yOffset;
    [SerializeField] Color _mpAdditionColor;
    [SerializeField] Color _mpReductionColor;
    [SerializeField] Color _comboTextColor;

    UIFloatingText[] _texts;
    Vector3 _offset;

    UIFloatingText GetInactiveText()
    {
        var text = _texts.Where(t => t != null && t.IsAvailable).FirstOrDefault();
        if (text != null)
            return text;

        Debug.Log("Ran out of floating texts");

        var newText = Instantiate(_damageTextPrefab, Vector3.zero, Quaternion.identity, transform);
        _texts = GetComponentsInChildren<UIFloatingText>();
        return newText;
    }

    void OnMPAbsorbed(BattleParticipant performer, BattleParticipant target, int amount)
    {
        var reductionText = GetInactiveText();
        _offset.x = UnityEngine.Random.Range(-1f, 1f);
        _offset.y = UnityEngine.Random.Range(-1f, 1f);
        reductionText.Play($"-{amount} MP", target.BodyMidPointPosition + _offset, _mpReductionColor);

        var additionText = GetInactiveText();
        _offset.x = UnityEngine.Random.Range(-1f, 1f);
        _offset.y = UnityEngine.Random.Range(-1f, 1f);
        additionText.Play($"+{amount} MP", performer.BodyMidPointPosition + _offset, _mpAdditionColor);
    }

    void OnHealthDamageReceived(BattleParticipant receiver, int damage, bool isCritical)
    {
        var text = GetInactiveText();
        _offset.x = 0f;
        _offset.y = UnityEngine.Random.Range(0.5f, 1.5f);

        text.Play(damage.ToString(), receiver.BodyMidPointPosition + _offset, Color.red, isCritical ? "critical" : "", Color.red);
    }

    void OnArmourDamageReceived(BattleParticipant receiver, int damage, bool isCritical)
    {
        var text = GetInactiveText();
        _offset.x = UnityEngine.Random.Range(-1.5f, -0.5f);
        _offset.y = UnityEngine.Random.Range(0f, 0.5f);

        text.Play(damage.ToString(), receiver.BodyMidPointPosition + _offset, Color.gray, isCritical ? "critical" : "", Color.grey);
    }

    void OnMissedAttackReceived(BattleParticipant target)
    {
        var text = GetInactiveText();
        _offset.x = UnityEngine.Random.Range(0f, 1f);
        _offset.y = UnityEngine.Random.Range(0f, 1f);

        text.Play("miss", target.BodyMidPointPosition + _offset, Color.grey);
    }

    void OnCriticalAttackReceived(BattleParticipant target)
    {
        var text = GetInactiveText();
        _offset.x = UnityEngine.Random.Range(0.5f, 2f);
        _offset.y = UnityEngine.Random.Range(0.5f, 1f);

        text.Play("critical", target.BodyMidPointPosition + _offset, Color.yellow);
    }

    void OnComboRequested(PartyMember partyMember1, PartyMember partyMember2)
    {
        var text = GetInactiveText();
        text.Play("combo", partyMember1.BodyMidPointPosition, _comboTextColor);
        
        text = GetInactiveText();
        text.Play("combo", partyMember2.BodyMidPointPosition, _comboTextColor);
    }

    void OnComboCancelled(PartyMember partyMember1, PartyMember partyMember2)
    {
        var text = GetInactiveText();
        text.Play("combo break", partyMember1.BodyMidPointPosition, _comboTextColor);
    }

    void OnComboEffectsGained(BattleParticipant battleParticipant, string effectsString)
    {
        var text = GetInactiveText();
        text.Play(effectsString, battleParticipant.BodyMidPointPosition, _comboTextColor);
    }

    void OnDestroy()
    {
        BattleEvents.HealthDamageReceived -= OnHealthDamageReceived;
        BattleEvents.ArmourDamageReceived -= OnArmourDamageReceived;
        BattleEvents.MissedAttackReceived -= OnMissedAttackReceived;
        BattleEvents.CriticalAttackReceived -= OnCriticalAttackReceived;
        BattleEvents.MPAbsorbed -= OnMPAbsorbed;
        BattleEvents.ComboRequested -= OnComboRequested;
        BattleEvents.ComboCancelled -= OnComboCancelled;
        BattleEvents.ComboEffectsGained -= OnComboEffectsGained;
    }

    void Awake()
    {
        _offset.y = _yOffset;
        _texts = GetComponentsInChildren<UIFloatingText>();

        // todo: maybe all these should just get the positions instead of the entire reference
        BattleEvents.HealthDamageReceived += OnHealthDamageReceived;
        BattleEvents.ArmourDamageReceived += OnArmourDamageReceived;
        BattleEvents.MissedAttackReceived += OnMissedAttackReceived;
        BattleEvents.CriticalAttackReceived += OnCriticalAttackReceived;  
        BattleEvents.MPAbsorbed += OnMPAbsorbed;    
        BattleEvents.ComboRequested += OnComboRequested;
        BattleEvents.ComboCancelled += OnComboCancelled;
        BattleEvents.ComboEffectsGained += OnComboEffectsGained;
    }
}
