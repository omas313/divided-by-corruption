using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBattleMenu : MonoBehaviour
{
    UIBattleMenuItem[] _items;
    CanvasGroup _canvasGroup;
    int _currentIndex;
    bool _isActive;
    private PartyMember _partyMember;
    private BattleActionPacket _currentBattleActionPacket;

    public void Hide() => _canvasGroup.alpha = 0f;

    public void Show() => _canvasGroup.alpha = 1f;

    void StartSelection()
    {
        StartCoroutine(StartSelectionAfterDelay(0.15f));
    }

    IEnumerator StartSelectionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        _currentIndex = 0;
        _isActive = true;
        foreach (var item in _items)
            item.gameObject.SetActive(true);
        UpdateActiveStates();
        Show();
    }
    
    void StopSelection()
    {
        Hide();
        _isActive = false;
    }

    void OnPartyMemberTurnStarted(PartyMember partyMember, BattleActionPacket battleActionPacket)
    {
        // todo: create party member actions in the future (when they have different actions)
        Show();
        _partyMember = partyMember;
        _currentBattleActionPacket = battleActionPacket;
    }

    void OnPartyMemberTurnEnded(PartyMember partyMember)
    {
        _partyMember = null;
        _currentBattleActionPacket = null;
        StopSelection();
    }

    void OnBattleActionSelectionRequested() => StartSelection();
    void OnActionBarCompleted() => Hide();

    void UpdateActiveStates()
    {
        for (var i = 0; i < _items.Length; i++)
            _items[i].SetActiveState(i == _currentIndex);
    }

    void GoToNextItem()
    {
        if (_currentIndex == _items.Length - 1)
            return;

        _currentIndex++;
        UpdateActiveStates();
        BattleAudioSource.Instance.PlaySelectSound();

    }

    void GoToPreviousItem()
    {
        if (_currentIndex == 0)
            return;

        _currentIndex--;
        UpdateActiveStates();
        BattleAudioSource.Instance.PlaySelectSound();

    }

    void ConfirmSelection()
    {
        var actionType = _items[_currentIndex].BattleActionType;

        switch (actionType)
        {
            case BattleActionType.Attack:
                var attackAction = new AttackAction(_partyMember, BattleActionType.Attack);
                attackAction.AttackDefinition = _partyMember.NormalAttackDefinition;
                _currentBattleActionPacket.BattleAction = attackAction;
                BattleUIEvents.InvokeEnemyTargetSelectionRequested();
                break;

            case BattleActionType.Special:
                _currentBattleActionPacket.BattleAction = new AttackAction(_partyMember, BattleActionType.Special);
                BattleUIEvents.InvokeSpecialAttackSelectionRequested();
                break;

            case BattleActionType.Defend:
                var defendAction = new DefendAction(_partyMember);
                defendAction.DefendDefinition = _partyMember.DefendDefinition;
                _currentBattleActionPacket.BattleAction = defendAction;
                break;

            case BattleActionType.Absorb:
                var absorbAction = new AbsorbAction(_partyMember);
                absorbAction.AbsorbDefinition = _partyMember.AbsorbDefinition;
                _currentBattleActionPacket.BattleAction = absorbAction;
                BattleUIEvents.InvokeEnemyTargetSelectionRequested();
                break;
            
            case BattleActionType.None:
            default:
                Debug.Log($"Error: unexpected battle action: {actionType}");
                break;
        }

        _isActive = false;
        SelectAndHideOthers();
        BattleAudioSource.Instance.PlaySelectSound();
    }

    void SelectAndHideOthers()
    {
        for (int i = 0; i < _items.Length; i++)
            if (i == _currentIndex)
                _items[i].SetSelected();
            else 
                _items[i].gameObject.SetActive(false);
    }

    void Update()
    {
        if (!_isActive)    
            return;

        if (Input.GetButtonDown("Down"))
            GoToNextItem();
        else if (Input.GetButtonDown("Up"))
            GoToPreviousItem();
        else if (Input.GetButtonDown("Confirm"))
            ConfirmSelection();
    }

    void OnDestroy()
    {
        BattleEvents.PartyMemberTurnStarted -= OnPartyMemberTurnStarted;
        BattleEvents.PartyMemberTurnEnded -= OnPartyMemberTurnEnded;
        BattleUIEvents.ActionBarCompleted -= OnActionBarCompleted;
        BattleUIEvents.BattleActionTypeSelectionRequested -= OnBattleActionSelectionRequested;
    }
    
    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _items = GetComponentsInChildren<UIBattleMenuItem>();

        BattleEvents.PartyMemberTurnStarted += OnPartyMemberTurnStarted;
        BattleEvents.PartyMemberTurnEnded += OnPartyMemberTurnEnded;
        BattleUIEvents.ActionBarCompleted += OnActionBarCompleted;
        BattleUIEvents.BattleActionTypeSelectionRequested += OnBattleActionSelectionRequested;
    }
}
