using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PositionSelectionManager<T> : MonoBehaviour where T : BattleParticipant
{
    protected Dictionary<Transform, T> positionsMap;
    protected List<Transform> activePositions;
    protected List<T> unselectables = new List<T>();

    [SerializeField] Transform[] _positions;
    [SerializeField] GameEvent _rightPressedEvent;
    [SerializeField] GameEvent _leftPressedEvent;
    [SerializeField] BattleParticipantMarker[] _targetMarkers;
    [SerializeField] Color _selectionMarkerColor;
    [SerializeField] Color _unselectableMarkerColor;

    bool _isActive;
    int _currentIndex;
    BattleActionPacket _currentBattleActionPacket;

    public void StartSelection()
    {
        StartCoroutine(StartSelectionInSeconds(0.1f));
    }

    protected abstract void OnBattleStarted(List<PartyMember> playerParty, List<Enemy> enemies);

    protected void RemovePositionOf(T participant)
    {
        Transform positionToRemove = null;

        foreach (var pair in positionsMap)
            if (pair.Value == participant)
                positionToRemove = pair.Key;
        
        activePositions.Remove(positionToRemove);
        positionsMap.Remove(positionToRemove);
    }

    protected void InitPositions(List<T> participants)
    {
        positionsMap = new Dictionary<Transform, T>();
        activePositions = new List<Transform>();

        for (var i = 0; i < participants.Count; i++)
        {
            var position = _positions[i];
            positionsMap[position] = participants[i];
            activePositions.Add(position);
            participants[i].InitPosition(activePositions[i].position);
        }

        activePositions = activePositions.OrderBy(p => p.name).ToList();

        SetSortingOrders();
    }

    protected void PlaceMarkerAt(Transform targetTransform)
    {
        var battleParticipant = positionsMap[targetTransform];

        var existingMarker = _targetMarkers.FirstOrDefault(m => m.ClientPosition == battleParticipant.CurrentPosition);
        if (existingMarker != null)
        {
            existingMarker.Show();
            existingMarker.SetColor(unselectables.Contains(battleParticipant) ? _unselectableMarkerColor : _selectionMarkerColor);
            return;
        }

        var inactiveMarker = _targetMarkers.FirstOrDefault(m => !m.IsActive);
        if (inactiveMarker == null)
        {
            Debug.Log("Error @ PositionSelectionManager: can't find inactive marker to place on target");
            return;
        }

        inactiveMarker.PlaceAt(targetTransform.position);
        inactiveMarker.SetColor(unselectables.Contains(battleParticipant) ? _unselectableMarkerColor : _selectionMarkerColor);
        BattleUIEvents.InvokeBattleParticipantHighlighted(battleParticipant);
    }

    protected void PlaceMarkerAt(BattleParticipant battleParticipant)
    {
        foreach (var kv in positionsMap)
            if (kv.Key == battleParticipant)
                PlaceMarkerAt(kv.Value);
    }
    
    protected void HideMarkers()
    {
        foreach (var marker in _targetMarkers)
            marker.Hide();
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
        switch (_currentBattleActionPacket.BattleAction.ActionDefinition.ActionTargetterType)
        {
            case ActionTargetterType.Single:
                var currentPositionTransform = activePositions[_currentIndex];
                HideMarkers();
                PlaceMarkerAt(currentPositionTransform);
                break;

            case ActionTargetterType.All:
                _currentIndex = 0;
                foreach (var positionTransform in activePositions)
                    PlaceMarkerAt(positionTransform);
                break;

            default: 
                break;
        }
    }

    void SetSortingOrders()
    {
        int i = 0;
        foreach (var position in activePositions)
            positionsMap[position].SetRendererSortingOrder(i++);
    }

    void GoBack()
    {
        if (_currentBattleActionPacket.BattleAction.BattleActionType == BattleActionType.Attack
            || _currentBattleActionPacket.BattleAction.BattleActionType == BattleActionType.Absorb
            || _currentBattleActionPacket.BattleAction.BattleActionType == BattleActionType.ComboRequest)
            BattleUIEvents.InvokeBattleActionTypeSelectionRequested();
        else if (_currentBattleActionPacket.BattleAction.BattleActionType == BattleActionType.Special)
            BattleUIEvents.InvokeSpecialAttackSelectionRequested();

        BattleUIEvents.InvokeTargetSelectionCancelled();
        BattleAudioSource.Instance.PlayUnselectSound();

        HideMarkers();
        _isActive = false;
    }

    void GoToPreviousPosition()
    {
        _currentIndex = Mathf.Max(_currentIndex - 1, 0);        
        SetCurrentPosition();
        BattleAudioSource.Instance.PlaySelectSound();
    }

    void GoToNextPosition()
    {
        _currentIndex = Mathf.Min(_currentIndex + 1, activePositions.Count - 1);              
        SetCurrentPosition();
        BattleAudioSource.Instance.PlaySelectSound();
    }

    void ConfirmCurrentSelection()
    {
        var selectedParticipant = positionsMap[activePositions[_currentIndex]];
        if (unselectables.Contains(selectedParticipant))
            return;

        switch (_currentBattleActionPacket.BattleAction.ActionDefinition.ActionTargetterType)
        {
            case ActionTargetterType.Single:
                _currentBattleActionPacket.SetTargets(selectedParticipant as BattleParticipant);
                break;

            case ActionTargetterType.All:
                var activeTargets = activePositions.Select(transform => positionsMap[transform] as BattleParticipant).ToList();
                _currentBattleActionPacket.SetTargets(activeTargets);
                break;

            default: 
                break;
        }
        
        HideMarkers();
        _isActive = false;
        unselectables.Clear();
        BattleAudioSource.Instance.PlaySelectSound();

        if (_currentBattleActionPacket.BattleAction is IActionBarAction)
            BattleUIEvents.InvokeActionBarRequested();
    }

    void RaiseRightPressedEvent()
    {
        if (_rightPressedEvent == null)
            return;
        
        _rightPressedEvent.Raise();
        HideMarkers();
        _isActive = false;
    }

    void RaiseLeftPressedEvent()
    {
        if (_leftPressedEvent == null)
            return;

        _leftPressedEvent.Raise();
        HideMarkers();
        _isActive = false;
    }

    void OnPartyMemberTurnStarted(PartyMember partyMember, BattleActionPacket battleActionPacket)
    {
        _currentBattleActionPacket = battleActionPacket;
    }

    void OnPartyMemberTurnEnded(PartyMember partyMember)
    {
        _currentBattleActionPacket = null;
    }

    void OnBattleParticipantsTargetted(List<BattleParticipant> battleParticipants)
    {
        foreach (BattleParticipant participant in battleParticipants)
            PlaceMarkerAt(participant);
    }

    void OnInvokeBattleParticipantTurnEnded(BattleParticipant obj)
    {
        HideMarkers();
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
        BattleEvents.BattleParticipantsTargetted -= OnBattleParticipantsTargetted;
        BattleEvents.BattleParticipantTurnEnded -= OnInvokeBattleParticipantTurnEnded;
    }

    protected virtual void Awake()
    {
        BattleEvents.BattleStarted += OnBattleStarted;
        BattleEvents.PartyMemberTurnStarted += OnPartyMemberTurnStarted;
        BattleEvents.PartyMemberTurnEnded += OnPartyMemberTurnEnded;
        BattleEvents.BattleParticipantsTargetted += OnBattleParticipantsTargetted;
        BattleEvents.BattleParticipantTurnEnded += OnInvokeBattleParticipantTurnEnded;
    }
}
