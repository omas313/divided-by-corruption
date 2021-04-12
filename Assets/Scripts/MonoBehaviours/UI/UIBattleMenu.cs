using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIBattleMenu : MonoBehaviour
{
    UIBattleMenuItem[] _items;
    CanvasGroup _canvasGroup;
    int _currentIndex;
    bool _isActive;
    PartyMember _partyMember;
    BattleActionPacket _currentBattleActionPacket;

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
                _currentBattleActionPacket.BattleAction = 
                    new AttackAction(_partyMember, BattleActionType.Attack, _partyMember.NormalAttackDefinition);

                if (_currentBattleActionPacket.HasTargetBeenSet())
                    BattleUIEvents.InvokeActionBarRequested();
                else
                    BattleUIEvents.InvokeEnemyTargetSelectionRequested(_currentBattleActionPacket.GetSelectableTargets());
                break;

            case BattleActionType.Special:
                _currentBattleActionPacket.BattleAction = 
                    new AttackAction(_partyMember, BattleActionType.Special);
                BattleUIEvents.InvokeSpecialAttackSelectionRequested();
                break;

            case BattleActionType.Defend:
                _currentBattleActionPacket.BattleAction = 
                    new DefendAction(_partyMember, _partyMember.DefendDefinition);;
                break;

            case BattleActionType.Absorb:
                _currentBattleActionPacket.BattleAction = 
                    new AbsorbAction(_partyMember, _partyMember.AbsorbDefinition);;
                BattleUIEvents.InvokeEnemyTargetSelectionRequested();
                break;

            case BattleActionType.ComboRequest:
                var comboRequestAction = new ComboRequestAction(
                    _partyMember, 
                    _currentBattleActionPacket.Combo, 
                    _partyMember.ComboRequestDefinition);
                _currentBattleActionPacket.BattleAction = comboRequestAction;
                _currentBattleActionPacket.Combo = comboRequestAction.Combo;
                BattleUIEvents.InvokePartyMemberTargetSelectionRequested(
                    _partyMember, 
                    unselectables: comboRequestAction.Combo.Participants);
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
