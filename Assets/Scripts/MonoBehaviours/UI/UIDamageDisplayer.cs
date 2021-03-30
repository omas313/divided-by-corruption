using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIDamageDisplayer : MonoBehaviour
{
    [SerializeField] UIFloatingText _damageTextPrefab;
    [SerializeField] float _yOffset;
    [SerializeField] Color _mpAdditionColor;
    [SerializeField] Color _mpReductionColor;

    UIFloatingText[] _texts;
    Vector3 _offset;

    void OnMPAbsorbed(BattleParticipant performer, BattleParticipant target, int amount)
    {
        var reductionText = GetInactiveText();
        _offset.x = UnityEngine.Random.Range(-1f, 1f);
        reductionText.Play($"-{amount} MP", target.CurrentPosition + _offset, _mpReductionColor);

        var additionText = GetInactiveText();
        _offset.x = UnityEngine.Random.Range(-1f, 1f);
        additionText.Play($"+{amount} MP", performer.CurrentPosition + _offset, _mpAdditionColor);
    }

    void OnHealthDamageReceived(BattleParticipant attacker, BattleParticipant receiver, BattleAttack attack)
    {
        var text = GetInactiveText();
        _offset.x = UnityEngine.Random.Range(-1f, 1f);

        text.Play(attack.Damage.ToString(), receiver.transform.position + _offset, attack.IsCritical ? Color.red : Color.white);
    }

    void OnAttackMissed(Vector3 position)
    {
        var text = GetInactiveText();
        _offset.x = UnityEngine.Random.Range(-1f, 1f);

        text.Play("miss", position + _offset, Color.grey);
    }

    void OnAttackCrit(Vector3 position)
    {
        var text = GetInactiveText();
        _offset.x = UnityEngine.Random.Range(-1f, 1f);

        text.Play("critical", position + _offset, Color.red);
    }

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

    void OnDestroy()
    {
        BattleEvents.HealthDamageReceived -= OnHealthDamageReceived;        
        BattleEvents.AttackMissedAt -= OnAttackMissed;      
        BattleEvents.AttackCritAt -= OnAttackCrit;      
        BattleEvents.MPAbsorbed -= OnMPAbsorbed;      
    }

    void Awake()
    {
        _offset.y = _yOffset;
        _texts = GetComponentsInChildren<UIFloatingText>();

        BattleEvents.HealthDamageReceived += OnHealthDamageReceived;      
        BattleEvents.AttackMissedAt += OnAttackMissed;      
        BattleEvents.AttackCritAt += OnAttackCrit;      
        BattleEvents.MPAbsorbed += OnMPAbsorbed;      
    }
}
