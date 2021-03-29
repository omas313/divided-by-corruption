using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UITurnDisplay : MonoBehaviour
{
    UIPortrait[] _portraits;
    CanvasGroup _canvasGroup;
    List<BattleParticipant> _battleParticipants;

    void Show() => _canvasGroup.alpha = 1f;
    void Hide() => _canvasGroup.alpha = 0f;

    void SetPortraits(BattleParticipant currentParticipant = null)
    {
        var battleParticipantIndex = currentParticipant != null ? _battleParticipants.IndexOf(currentParticipant) : 0;
        for (var i = 0; i < _portraits.Length; i++)
        {
            var portrait = _portraits[i];
            portrait.Init(_battleParticipants[battleParticipantIndex]);

            battleParticipantIndex = (battleParticipantIndex + 1) % _battleParticipants.Count;
        }

        Show();
    }

    void HighlightBattleParticipant(BattleParticipant battleParticipant)
    {
        foreach (var portrait in _portraits)
            if (portrait.IsPortraitFor(battleParticipant))
                portrait.SetHighlightedState(true);
            else
                portrait.SetHighlightedState(false);
    }
    

    void UnhighlightAllPortraits()
    {
        foreach (var portrait in _portraits)
            portrait.SetHighlightedState(false);
    }

    void OnBattleParticipantsUpdated(List<BattleParticipant> battleParticipants)
    {
        _battleParticipants = battleParticipants;

        SetPortraits();
    }

    void OnBattleParticipantHighlighted(BattleParticipant battleParticipant) => HighlightBattleParticipant(battleParticipant);

    void OnEnemySelectedTarget(List<BattleParticipant> battleParticipants)
    {
        foreach (var battleParticipant in battleParticipants)
            HighlightBattleParticipant(battleParticipant);
    }

    void OnBattleParticipantTurnStarted(BattleParticipant battleParticipant)
    {
        SetPortraits(battleParticipant);
        Show();
    }

    void OnBattleParticipantTurnEnded(BattleParticipant battleParticipant) => UnhighlightAllPortraits();
    
    void OnSpecialAttackSelectionRequested() => UnhighlightAllPortraits();

    void OnBattleActionTypeSelectionRequested() => UnhighlightAllPortraits();
    
    void OnActionBarRequested() => Hide();

    void OnDestroy()
    {
        BattleEvents.BattleParticipantsUpdated -= OnBattleParticipantsUpdated;
        BattleEvents.BattleParticipantTurnStarted -= OnBattleParticipantTurnStarted;
        BattleEvents.BattleParticipantTurnEnded -= OnBattleParticipantTurnEnded;
        BattleEvents.EnemySelectedTargets -= OnEnemySelectedTarget;
        BattleUIEvents.BattleParticipantHighlighted -= OnBattleParticipantHighlighted;
        BattleUIEvents.SpecialAttackSelectionRequested -= OnSpecialAttackSelectionRequested;
        BattleUIEvents.BattleActionTypeSelectionRequested -= OnBattleActionTypeSelectionRequested;
        BattleUIEvents.ActionBarRequested -= OnActionBarRequested;
    }

    void Awake()
    {
        _portraits = GetComponentsInChildren<UIPortrait>();
        _canvasGroup = GetComponent<CanvasGroup>();

        Hide();

        BattleEvents.BattleParticipantsUpdated += OnBattleParticipantsUpdated;
        BattleEvents.BattleParticipantTurnStarted += OnBattleParticipantTurnStarted;
        BattleEvents.BattleParticipantTurnEnded += OnBattleParticipantTurnEnded;
        BattleEvents.EnemySelectedTargets += OnEnemySelectedTarget;
        BattleUIEvents.BattleParticipantHighlighted += OnBattleParticipantHighlighted;
        BattleUIEvents.SpecialAttackSelectionRequested += OnSpecialAttackSelectionRequested;
        BattleUIEvents.BattleActionTypeSelectionRequested += OnBattleActionTypeSelectionRequested;
        BattleUIEvents.ActionBarRequested += OnActionBarRequested;
    }
}
