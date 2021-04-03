using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnemyStatusBar : MonoBehaviour
{
    [SerializeField] Enemy _source;
    [SerializeField] RectTransform _healthRect;
    [SerializeField] RectTransform _armourRect;

    CanvasGroup _canvasGroup;
    float _totalWidth;

    void Show() => _canvasGroup.alpha = 1f;
    void Hide() => _canvasGroup.alpha = 0f;

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

    void OnBattleParticipantHighlighted(BattleParticipant battleParticipant)
    {
        if (battleParticipant is Enemy && battleParticipant == _source)
            Show();
        else
            Hide();
    }

    void OnBattleParticipantsHighlighted(List<BattleParticipant> battleParticipants)
    {
        if (battleParticipants.Contains(_source))
            Show();
        else
            Hide();
    }

    void OnRequestedActionBar() => Hide();
    void OnTargetSelectionCancelled() => Hide();
    void OnBattleParticipantTurnStarted(BattleParticipant battleParticipant) => Hide();
    void OnBattleParticipantTurnEnded(BattleParticipant battleParticipant) => Hide();

    void OnBattleParticipantsTargetted(List<BattleParticipant> battleParticipants)
    {
        foreach (var battleParticipant in battleParticipants)
            if (battleParticipant == _source)
                Show();
    }

    void OnDestroy()
    {
        BattleEvents.EnemyArmourChanged -= OnArmourChanged;
        BattleEvents.EnemyHealthChanged -= OnHealthChanged;
        BattleEvents.BattleParticipantsTargetted -= OnBattleParticipantsTargetted;
        BattleEvents.BattleParticipantTurnStarted -= OnBattleParticipantTurnStarted;
        BattleEvents.BattleParticipantTurnEnded -= OnBattleParticipantTurnEnded;

        BattleUIEvents.BattleParticipantHighlighted -= OnBattleParticipantHighlighted;
        BattleUIEvents.BattleParticipantsHighlighted -= OnBattleParticipantsHighlighted;
        BattleUIEvents.ActionBarRequested -= OnRequestedActionBar;
        BattleUIEvents.TargetSelectionCancelled -= OnTargetSelectionCancelled;
    }

    void Awake()
    {
        _totalWidth = _healthRect.sizeDelta.x;
        _canvasGroup = GetComponent<CanvasGroup>();

        Hide();

        BattleEvents.EnemyArmourChanged += OnArmourChanged;
        BattleEvents.EnemyHealthChanged += OnHealthChanged;
        BattleEvents.BattleParticipantsTargetted += OnBattleParticipantsTargetted;
        BattleEvents.BattleParticipantTurnStarted += OnBattleParticipantTurnStarted;
        BattleEvents.BattleParticipantTurnEnded += OnBattleParticipantTurnEnded;

        BattleUIEvents.BattleParticipantsHighlighted += OnBattleParticipantsHighlighted;
        BattleUIEvents.BattleParticipantHighlighted += OnBattleParticipantHighlighted;
        BattleUIEvents.ActionBarRequested += OnRequestedActionBar;
        BattleUIEvents.TargetSelectionCancelled += OnTargetSelectionCancelled;
    }
}
