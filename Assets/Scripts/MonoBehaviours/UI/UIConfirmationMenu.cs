using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIConfirmationMenu : MonoBehaviour
{
    [SerializeField] UIBattleMenuItem _yesItem;
    [SerializeField] UIBattleMenuItem _noItem;

    UIBattleMenuItem[] _items;
    CanvasGroup _canvasGroup;
    int _currentIndex;
    bool _isActive;

    public void StartSelection()
    {
        StartCoroutine(StartSelectionAfterDelay(0.15f));
    }

    IEnumerator StartSelectionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _currentIndex = 0;
        _isActive = true;
        UpdateActiveStates();
        Show();
        GoToNextItem();
    }

    void Hide() => _canvasGroup.alpha = 0f;
    
    void Show() => _canvasGroup.alpha = 1f;

    void UpdateActiveStates()
    {
        for (var i = 0; i < _items.Length; i++)
            _items[i].SetActiveState(i == _currentIndex);
    }

    void Update()
    {
        if (!_isActive)    
            return;

        if (Input.GetButtonDown("Right"))
            GoToNextItem();
        else if (Input.GetButtonDown("Left"))
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
        _items[_currentIndex].RaiseEvent();

        // if (_items[_currentIndex] == _noItem)
        //     BattleEvents.InvokeCommandsNotConfirmed();
        // else
        //     BattleEvents.InvokeCommandsConfirmed();

        _isActive = false;
        Hide();
    }

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _items = new UIBattleMenuItem[] { _noItem, _yesItem };
        // BattleEvents.RequestedCommandsConfirmation += StartSelection;

        Hide();
    }

    void OnDestroy()
    {
        // BattleEvents.RequestedCommandsConfirmation -= StartSelection;
    }
}
