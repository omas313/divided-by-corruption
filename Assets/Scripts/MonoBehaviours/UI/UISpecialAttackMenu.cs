using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpecialAttackMenu : MonoBehaviour
{
    [SerializeField] UISpecialAttackMenuItem _itemsPrefab;
    [SerializeField] RectTransform _itemsParent;

    UISpecialAttackMenuItem[] _items;
    CanvasGroup _canvasGroup;
    int _currentIndex;
    bool _isActive;
    private BattleAction _currentBattleAction;

    public void Hide() => _canvasGroup.alpha = 0f;

    public void Show() => _canvasGroup.alpha = 1f;

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
    }
    
    void StopSelection()
    {
        Hide();
        _isActive = false;
    }

    void CreateAttacks(PartyMember partyMember)
    {
        DestroyOldItems();

        var attackDefinitions = partyMember.SpecialAttacksDefinitions;
        _items = new UISpecialAttackMenuItem[attackDefinitions.Count];

        for (int i = 0; i < attackDefinitions.Count; i++)
        {
            _items[i] = Instantiate(_itemsPrefab, _itemsParent.transform.position, Quaternion.identity, _itemsParent);
            _items[i].Init(attackDefinitions[i], isSelectable: partyMember.CharacterStats.HasEnoughMP(attackDefinitions[i].MPCost));
        }

        UpdateActiveStates();
    }

    void DestroyOldItems()
    {
        if (_items == null)
            return;

        foreach (var item in _items)
            Destroy(item.gameObject);
    }

    void OnPartyMemberTurnStarted(PartyMember partyMember, BattleAction battleAction)
    {
        _currentBattleAction = battleAction;
        CreateAttacks(partyMember);
    }

    void OnPartyMemberTurnEnded(PartyMember partyMember)
    {
        _currentBattleAction = null;
        StopSelection();
    }

    void OnSpecialAttackSelectionRequested()
    {
        StartSelection();
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

    void GoBack()
    {
        _currentBattleAction.AttackDefinition = null;
        BattleUIEvents.InvokeBattleActionTypeSelectionRequested();
        StopSelection();
    }

    void ConfirmSelection()
    {
        if (!_items[_currentIndex].IsSelectable)
            return;

        _currentBattleAction.AttackDefinition = _items[_currentIndex].AttackDefinition;
        BattleUIEvents.InvokeEnemyTargetSelectionRequested();

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
        else if (Input.GetButtonDown("Back"))
            GoBack();
    }

    void OnDestroy()
    {
        BattleEvents.PartyMemberTurnStarted -= OnPartyMemberTurnStarted;
        BattleEvents.PartyMemberTurnEnded -= OnPartyMemberTurnEnded;
        BattleUIEvents.SpecialAttackSelectionRequested -= OnSpecialAttackSelectionRequested;
    }
    
    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        BattleEvents.PartyMemberTurnStarted += OnPartyMemberTurnStarted;
        BattleEvents.PartyMemberTurnEnded += OnPartyMemberTurnEnded;
        BattleUIEvents.SpecialAttackSelectionRequested += OnSpecialAttackSelectionRequested;
    }
}
