using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnemyHealthBar : MonoBehaviour
{
    [SerializeField] RectTransform _healthRect;
    [SerializeField] RectTransform _armourRect;
    [SerializeField] Enemy _source;

    float _totalWidth;

    void OnArmourChanged(Enemy enemy, int currentValue, int baseValue)
    {
        if (enemy != _source)
            return;
            
        var size = new Vector2((currentValue / (float) baseValue) * _totalWidth, _armourRect.sizeDelta.y);
        _armourRect.sizeDelta = size;
    }

    void OnHealthChanged(Enemy enemy, int currentValue, int baseValue)
    {
        if (enemy != _source)
            return;
            
        var size = new Vector2((currentValue / (float) baseValue) * _totalWidth, _healthRect.sizeDelta.y);
        _healthRect.sizeDelta = size;
    }

    void Awake()
    {
        _totalWidth = _healthRect.sizeDelta.x;

        BattleEvents.EnemyArmourChanged += OnArmourChanged;
        BattleEvents.EnemyHealthChanged += OnHealthChanged;
    }

    void OnDestroy()
    {
        BattleEvents.EnemyArmourChanged -= OnArmourChanged;
        BattleEvents.EnemyHealthChanged -= OnHealthChanged;
    }
}
