using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// rename to enemy status bar
public class UIEnemyHealthBar : MonoBehaviour
{
    [SerializeField] Enemy _source;
    [SerializeField] RectTransform _healthRect;
    [SerializeField] CanvasGroup _armourCanvasGroup;
    [SerializeField] GameObject _armourPrefab;

    List<GameObject> _armourPieces = new List<GameObject>();
    CanvasGroup _canvasGroup;
    float _totalWidth;

    void Show() => _canvasGroup.alpha = 1f;
    void Hide() => _canvasGroup.alpha = 0f;

    void SetArmour()
    {
        for (var i = 0; i < _source.EnemyStats.CurrentArmour; i++)
            _armourPieces.Add(Instantiate(_armourPrefab, _armourCanvasGroup.transform.position, Quaternion.identity, _armourCanvasGroup.transform));
    }

    void OnArmourChanged(Enemy enemy, int currentValue, int baseValue)
    {
        if (enemy != _source)
            return;
        
        for (var i = 0; i < _armourPieces.Count; i++)
            if (i >= currentValue)
                StartCoroutine(PlayAndDeleteArmourPiece(_armourPieces[i]));
    }

    IEnumerator PlayAndDeleteArmourPiece(GameObject armourPiece)
    {
        var animation = armourPiece.GetComponent<Animation>();
        animation.Play();

        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !animation.isPlaying);
        
        _armourPieces.Remove(armourPiece);
        Destroy(armourPiece);
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

        SetArmour();
        Hide();

        BattleEvents.EnemyArmourChanged += OnArmourChanged;
        BattleEvents.EnemyHealthChanged += OnHealthChanged;
        BattleEvents.BattleParticipantsTargetted += OnBattleParticipantsTargetted;
        BattleEvents.BattleParticipantTurnStarted += OnBattleParticipantTurnStarted;
        BattleUIEvents.BattleParticipantsHighlighted += OnBattleParticipantsHighlighted;
        BattleEvents.BattleParticipantTurnEnded += OnBattleParticipantTurnEnded;
        BattleUIEvents.BattleParticipantHighlighted += OnBattleParticipantHighlighted;
        BattleUIEvents.ActionBarRequested += OnRequestedActionBar;
        BattleUIEvents.TargetSelectionCancelled += OnTargetSelectionCancelled;
    }
}
