using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIDamageDisplayer : MonoBehaviour
{
    [SerializeField] UIFloatingText _damageTextPrefab;
    [SerializeField] float _yOffset;

    UIFloatingText[] _texts;
    Vector3 _offset;

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
        BattleEvents.AttackMissed -= OnAttackMissed;      
        BattleEvents.AttackCrit -= OnAttackCrit;      
    }

    void Awake()
    {
        _offset.y = _yOffset;
        _texts = GetComponentsInChildren<UIFloatingText>();

        BattleEvents.HealthDamageReceived += OnHealthDamageReceived;      
        BattleEvents.AttackMissed += OnAttackMissed;      
        BattleEvents.AttackCrit += OnAttackCrit;      
    }
}
