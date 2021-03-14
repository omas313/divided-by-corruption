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

    public void Hide() => _canvasGroup.alpha = 0f;

    public void Show() => _canvasGroup.alpha = 1f;

    public void StartSelection()
    {
        StartCoroutine(StartSelectionAfterDelay(0.15f));
    }

    IEnumerator StartSelectionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        BattleEvents.InvokeActionTypeSelected(BattleActionType.None);
        _currentIndex = 0;
        _isActive = true;
        UpdateActiveStates();
        Show();
    }
    
    void StopSelection()
    {
        Hide();
        _isActive = false;
    }

    void OnPartyMemberTurnStarted(PartyMember partyMember)
    {
        StartSelection();
    }

    void OnPartyMemberTurnEnded(PartyMember partyMember)
    {
        StopSelection();
    }

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
    }

    void GoToPreviousItem()
    {
        if (_currentIndex == 0)
            return;

        _currentIndex--;
        UpdateActiveStates();
    }

    void ConfirmSelection()
    {
        _items[_currentIndex].RaiseEvent();
        _isActive = false;
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
    }
    
    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _items = GetComponentsInChildren<UIBattleMenuItem>();

        BattleEvents.PartyMemberTurnStarted += OnPartyMemberTurnStarted;
        BattleEvents.PartyMemberTurnEnded += OnPartyMemberTurnEnded;
    }
}
