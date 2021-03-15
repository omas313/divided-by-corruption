using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PositionSelectionManager<T> : MonoBehaviour where T : BattleParticipant
{
    [SerializeField] Transform[] _positions;
    [SerializeField] GameEvent _rightPressedEvent;
    [SerializeField] GameEvent _leftPressedEvent;
    [SerializeField] BattleParticipantMarker _targetMarker;
    [SerializeField] Color _selectionMarkerColor;

    List<Transform> _activePositions;
    Dictionary<Transform, T> _positionsMap;
    bool _isActive;
    int _currentIndex;
    private BattleAction _currentBattleAction;

    protected void StartSelection()
    {
        StartCoroutine(StartSelectionInSeconds(0.1f));
    }

    protected abstract void OnBattleStarted(List<PartyMember> playerParty, List<Enemy> enemies);

    protected void RemovePositionOf(T participant)
    {
        Transform positionToRemove = null;

        foreach (var pair in _positionsMap)
            if (pair.Value == participant)
                positionToRemove = pair.Key;
        
        _activePositions.Remove(positionToRemove);
        _positionsMap.Remove(positionToRemove);
    }

    protected void InitPositions(List<T> participants)
    {
        _positionsMap = new Dictionary<Transform, T>();
        _activePositions = new List<Transform>();

        for (var i = 0; i < participants.Count; i++)
        {
            var position = _positions[i];
            _positionsMap[position] = participants[i];
            _activePositions.Add(position);
            participants[i].Init(_activePositions[i].position);
        }

        _activePositions = _activePositions.OrderBy(p => p.name).ToList();

        SetSortingOrders();
    }
    
    IEnumerator StartSelectionInSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        _currentIndex = 0;
        _isActive = true;
        SetCurrentPosition();
    }

    void SetCurrentPosition()
    {
        var position = _activePositions[_currentIndex].position;
        _targetMarker.SetColor(_selectionMarkerColor);
        _targetMarker.PlaceAt(position);
    }

    void SetSortingOrders()
    {
        int i = 0;
        foreach (var position in _activePositions)
            _positionsMap[position].SetRendererSortingOrder(i++);
    }
    
    void GoBack()
    {
        if (_currentBattleAction.BattleActionType == BattleActionType.Attack)
            BattleUIEvents.InvokeBattleActionTypeSelectionRequested();
        else if (_currentBattleAction.BattleActionType == BattleActionType.Special)
            BattleUIEvents.InvokeSpecialAttackSelectionRequested();

        _targetMarker.Hide();
        _isActive = false;
    }

    void GoToPreviousPosition()
    {
        _currentIndex = Mathf.Max(_currentIndex - 1, 0);        
        SetCurrentPosition();
    }

    void GoToNextPosition()
    {
        _currentIndex = Mathf.Min(_currentIndex + 1, _activePositions.Count - 1);              
        SetCurrentPosition();
    }

    void ConfirmCurrentSelection()
    {
        _targetMarker.Hide();
        _isActive = false;

        _currentBattleAction.Target = _positionsMap[_activePositions[_currentIndex]];
        BattleUIEvents.InvokeRequestedActionBar();
    }

    void RaiseRightPressedEvent()
    {
        if (_rightPressedEvent == null)
            return;
        
        _rightPressedEvent.Raise();
        _isActive = false;
    }

    void RaiseLeftPressedEvent()
    {
        if (_leftPressedEvent == null)
            return;

        _leftPressedEvent.Raise();
        _isActive = false;
    }

    void OnPartyMemberTurnStarted(PartyMember partyMember, BattleAction battleAction)
    {
        _currentBattleAction = battleAction;
    }

    void OnPartyMemberTurnEnded(PartyMember partyMember)
    {
        _currentBattleAction = null;
    }

    void Update()
    {
        if (!_isActive)    
            return;

        if (Input.GetButtonDown("Up"))
            GoToPreviousPosition();
        else if (Input.GetButtonDown("Down"))
            GoToNextPosition();
        else if (Input.GetButtonDown("Left"))
            RaiseLeftPressedEvent();
        else if (Input.GetButtonDown("Right"))
            RaiseRightPressedEvent();
        else if (Input.GetButtonDown("Confirm"))
            ConfirmCurrentSelection();    
        else if (Input.GetButtonDown("Back"))
            GoBack();
    }

    protected virtual void OnDestroy()
    {
        BattleEvents.BattleStarted -= OnBattleStarted;
        BattleEvents.PartyMemberTurnStarted -= OnPartyMemberTurnStarted;
        BattleEvents.PartyMemberTurnEnded -= OnPartyMemberTurnEnded;
    }

    protected virtual void Awake()
    {
        BattleEvents.BattleStarted += OnBattleStarted;
        BattleEvents.PartyMemberTurnStarted += OnPartyMemberTurnStarted;
        BattleEvents.PartyMemberTurnEnded += OnPartyMemberTurnEnded;
    }
}
