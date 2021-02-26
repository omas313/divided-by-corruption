using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIDamageDisplayer : MonoBehaviour
{
    [SerializeField] UIFloatingText _damageTextPrefab;

    UIFloatingText[] _texts;

    void OnHealthDamageReceived(Vector3 position, int damage, Color color)
    {
        var text = GetInactiveText();
        text.Play(damage.ToString(), position, color);
    }

    void OnArmourDamageReceived(Vector3 position, int damage, Color color)
    {
        var text = GetInactiveText();
        text.Play(damage.ToString(), position, color);
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
        BattleEvents.ArmourDamageReceived -= OnArmourDamageReceived;
    }

    void Awake()
    {
        _texts = GetComponentsInChildren<UIFloatingText>();

        BattleEvents.HealthDamageReceived += OnHealthDamageReceived;        
        BattleEvents.ArmourDamageReceived += OnArmourDamageReceived;
    }
}
