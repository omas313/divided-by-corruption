using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PositionSelectionManager<T> : MonoBehaviour where T : BattleParticipant
{
    [SerializeField] Transform[] _positions;
    [SerializeField] GameEvent _rightPressedEvent;
    [SerializeField] GameEvent _leftPressedEvent;
    [SerializeField] BattleParticipantMarker[] _targetMarkers;
    [SerializeField] Color _selectionMarkerColor;

    List<Transform> _activePositions;
    Dictionary<Transform, T> _positionsMap;
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
            participants[i].InitPosition(_activePositions[i].position);
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
        switch (_currentBattleActionPacket.BattleAction.ActionDefinition.ActionTargetterType)
        {
            case ActionTargetterType.Single:
                _targetMarkers[0].SetColor(_selectionMarkerColor);
                _targetMarkers[0].PlaceAt(_activePositions[_currentIndex].position);

                BattleUIEvents.InvokeBattleParticipantHighlighted(_positionsMap[_activePositions[_currentIndex]] as BattleParticipant);
                break;

            case ActionTargetterType.All:
                _currentIndex = 0;
                var markerIndex = 0;
                foreach (var positionTransform in _activePositions)
                {
                    _targetMarkers[markerIndex].SetColor(_selectionMarkerColor);
                    _targetMarkers[markerIndex].PlaceAt(positionTransform.position);
                    markerIndex++;
                    
                    BattleUIEvents.InvokeBattleParticipantsHighlighted(_positionsMap.Values.ToList<BattleParticipant>());
                }
                break;

            default: 
                break;
        }
    }

    void SetSortingOrders()
    {
        int i = 0;
        foreach (var position in _activePositions)
            _positionsMap[position].SetRendererSortingOrder(i++);
    }
    
    void HideMarkers()
    {
        foreach (var marker in _targetMarkers)
            marker.Hide();
    }

    void GoBack()
    {
        if (_currentBattleActionPacket.BattleAction.BattleActionType == BattleActionType.Attack)
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
        _currentIndex = Mathf.Min(_currentIndex + 1, _activePositions.Count - 1);              
        SetCurrentPosition();
        BattleAudioSource.Instance.PlaySelectSound();
    }

    void ConfirmCurrentSelection()
    {
        HideMarkers();
        _isActive = false;

        switch (_currentBattleActionPacket.BattleAction.ActionDefinition.ActionTargetterType)
        {
            case ActionTargetterType.Single:
                _currentBattleActionPacket.BattleAction.Targets.Add(_positionsMap[_activePositions[_currentIndex]]);
                break;

            case ActionTargetterType.All:
                foreach (var positionTransform in _activePositions)
                    _currentBattleActionPacket.BattleAction.Targets.Add(_positionsMap[positionTransform]);
                break;

            default: 
                break;
        }
        
        BattleUIEvents.InvokeActionBarRequested();
        BattleAudioSource.Instance.PlaySelectSound();
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
