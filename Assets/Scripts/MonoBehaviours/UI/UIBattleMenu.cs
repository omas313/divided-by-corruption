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


    void OnPartyMembersUpdated(List<PartyMember> party, PartyMember currentPartyMember)
    {
        if (currentPartyMember == null)
        {
            Hide();
            _isActive = false;
            return;
        }

        Show();
        _isActive = true;
        _currentIndex = 0;

        UpdateActiveStates();
    }

    void UpdateActiveStates()
    {
        for (var i = 0; i < _items.Length; i++)
            _items[i].SetActiveState(i == _currentIndex);
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

    }

    void Start()
    {
        BattleEvents.PartyMembersUpdated += OnPartyMembersUpdated;
    }

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _items = GetComponentsInChildren<UIBattleMenuItem>();
    }

    void OnDestroy()
    {
        BattleEvents.PartyMembersUpdated -= OnPartyMembersUpdated;
    }
}
