using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public BattleParticipant CurrentBattleParticipant => _battleParticipants[_currentIndex];
    public int CurrentIndex => _currentIndex;
    
    [SerializeField] float _delayBetweenTurns = 2f;

    PartyMember CurrentPartyMember => CurrentBattleParticipant as PartyMember;
    Enemy CurrentEnemy => CurrentBattleParticipant as Enemy;
    bool ShouldHandleComboTurns => _currentCombo != null && _currentCombo.HasStarted && _currentCombo.HasParticipants;
    List<BattleParticipant> _battleParticipants;
    List<PartyMember> _activeParty;
    List<Enemy> _activeEnemies;
    int _currentIndex;
    CurrentActorMarker _currentActorMarker;
    Combo _currentCombo;

    public void Init(List<BattleParticipant> battleParticipants, List<PartyMember> activeParty, List<Enemy> activeEnemies)
    {
        _battleParticipants = battleParticipants;
        _activeParty = activeParty;
        _activeEnemies = activeEnemies;
    }
    
    public void IncrementCurrentIndex() => _currentIndex = (_currentIndex + 1) % _battleParticipants.Count;
    public void SetCurrentIndex(int index) => _currentIndex = index;

    public IEnumerator ManageTurn()
    {
        if (!ShouldHandleComboTurns)
            yield return ManageNextTurn();
        else
            yield return ManageComboTurn();
    }

    IEnumerator ManageNextTurn()
    {
        BattleEvents.InvokeBattleParticipantTurnStarted(CurrentBattleParticipant);

        if (CurrentBattleParticipant is Enemy)
            yield return ManageEnemyTurn();
        else
            yield return ManagePartyMemberTurn(CurrentPartyMember);

        BattleEvents.InvokeBattleParticipantTurnEnded(CurrentBattleParticipant);

        IncrementCurrentIndex();

        yield return new WaitForSeconds(_delayBetweenTurns);
    }

    IEnumerator ManageEnemyTurn()
    {
        BattleEvents.InvokeEnemyTurnStarted(CurrentEnemy);

        _currentActorMarker.Mark(CurrentEnemy.TopMarkerTransform);
        yield return new WaitForSeconds(1f);
        _currentActorMarker.Hide();

        yield return CurrentEnemy.GetNextAction(_activeParty, _activeEnemies).PerformAction(_activeParty, _activeEnemies);

        CurrentEnemy.EndTurn();
        BattleEvents.InvokeEnemyTurnEnded(CurrentEnemy);
    }

    IEnumerator ManagePartyMemberTurn(PartyMember partyMember)
    {
        var battleActionPacket = new BattleActionPacket() { Combo = _currentCombo };

        BattleEvents.InvokePartyMemberTurnStarted(partyMember, battleActionPacket);
        BattleUIEvents.InvokeBattleActionTypeSelectionRequested();
        _currentActorMarker.Mark(partyMember.TopMarkerTransform);

        yield return new WaitUntil(() => battleActionPacket.HasValidAction);
        
        _currentActorMarker.Hide();
        yield return battleActionPacket.BattleAction.PerformAction(_activeParty, _activeEnemies);

        yield return new WaitForSeconds(0.5f);
        yield return HandleComboStatus(battleActionPacket);

        partyMember.EndTurn();
        BattleEvents.InvokePartyMemberTurnEnded(partyMember);
    }

    IEnumerator ManageComboTurn()
    {
        var currentComboPerformer = _currentCombo.NextParticipant;

        yield return ManagePartyMemberTurn(currentComboPerformer);

        _currentCombo.RemoveParticipant(currentComboPerformer);
        BattleEvents.InvokeComboParticipantsChanged(_currentCombo.Participants.ToList());

        if (_currentCombo.HasFinished)
            yield return EndCombo();
    }

    IEnumerator HandleComboStatus(BattleActionPacket battleActionPacket)
    {
        var partyMember = battleActionPacket.BattleAction.Performer as PartyMember;

        if (HasNoComboToHandle(battleActionPacket))
            yield break;

        if (HasComboBeenRequested(battleActionPacket))
        {
            _currentCombo = battleActionPacket.Combo;
            yield break;
        }

        if (!IsPartyMemberParticipatingInACombo(partyMember))
            yield break;

        var battleActionType = battleActionPacket.BattleAction.BattleActionType;
        
        if (_currentCombo.ShouldPerformComboTrial(partyMember, battleActionType))
        {
            yield return PerformComboTrial(battleActionPacket, _activeParty, _activeEnemies);
            
            _currentCombo.RemoveParticipant(partyMember);
            BattleEvents.InvokeComboParticipantsChanged(_currentCombo.Participants);
            yield break;
        }

        if (_currentCombo.ShouldBreakCombo(partyMember, battleActionType))
            yield return BreakCombo(battleActionPacket, partyMember);
    }

    bool HasNoComboToHandle(BattleActionPacket battleActionPacket) 
        => _currentCombo == null && !battleActionPacket.HasCombo;
    bool IsPartyMemberParticipatingInACombo(PartyMember partyMember) 
        => _currentCombo != null && _currentCombo.IsParticipant(partyMember);
    bool HasComboBeenRequested(BattleActionPacket battleActionPacket) 
        => _currentCombo == null && battleActionPacket.HasCombo;

    IEnumerator BreakCombo(BattleActionPacket battleActionPacket, PartyMember partyMember)
    {
        BattleEvents.InvokeComboBroken(partyMember);
        _currentCombo.Clear();
        _currentCombo = null;

        yield return new WaitForSeconds(1f);
    }

    IEnumerator EndCombo()
    {
        _currentCombo.Clear();
        _currentCombo = null;
        BattleEvents.InvokeComboFinished();

        // do some combo feedback animations
        yield return null;
    }

    IEnumerator PerformComboTrial(BattleActionPacket firstAttackPacket, List<PartyMember> party, List<Enemy> enemies)
    {
        var partyMember = firstAttackPacket.BattleAction.Performer as PartyMember;

        _currentActorMarker.Mark(partyMember.TopMarkerTransform);

        var targets = firstAttackPacket.BattleAction.Targets;
        var comboTrialAction = new ComboTrialAction(
            partyMember, 
            firstAttackPacket.BattleAction.ActionDefinition as AttackDefinition, 
            _currentCombo);

        var battleActionPacket = new BattleActionPacket()
        {
            BattleAction = comboTrialAction,
            Combo = _currentCombo
        };
        battleActionPacket.SetTargets(targets);

        BattleEvents.InvokeBattleParticipantsTargetted(targets);
        BattleEvents.InvokeComboTrialRequested(battleActionPacket);

        yield return new WaitUntil(() => battleActionPacket.HasValidAction);
        yield return battleActionPacket.BattleAction.PerformAction(party, enemies);

        if (!comboTrialAction.IsSuccess)
            _currentCombo = null;

        _currentActorMarker.Hide();
    }

    void Awake()
    {
        _currentActorMarker = GetComponentInChildren<CurrentActorMarker>();
    }
}
