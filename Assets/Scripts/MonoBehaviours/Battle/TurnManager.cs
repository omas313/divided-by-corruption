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

        if (_currentCombo != null)
        {
            _currentCombo.RemoveParticipant(currentComboPerformer);
            BattleEvents.InvokeComboParticipantsChanged(_currentCombo.Participants.ToList());
        }

        if (_currentCombo != null && _currentCombo.HasFinished)
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

        var battleAction = battleActionPacket.BattleAction;
        
        if (_currentCombo.ShouldPerformComboTrial(partyMember, battleAction))
        {
            yield return PerformComboTrial(battleActionPacket, _activeParty, _activeEnemies);

            if (_currentCombo.IsBroken)
            {
                yield return BreakCombo(partyMember);
                yield break;
            }
            
            _currentCombo.RemoveParticipant(partyMember);
            BattleEvents.InvokeComboParticipantsChanged(_currentCombo.Participants);
            yield break;
        }

        if (_currentCombo.ShouldBreakCombo(partyMember, battleAction))
            yield return BreakCombo(partyMember);
    }

    bool HasNoComboToHandle(BattleActionPacket battleActionPacket) 
        => _currentCombo == null && !battleActionPacket.HasCombo;
    bool IsPartyMemberParticipatingInACombo(PartyMember partyMember) 
        => _currentCombo != null && _currentCombo.IsParticipant(partyMember);
    bool HasComboBeenRequested(BattleActionPacket battleActionPacket) 
        => _currentCombo == null && battleActionPacket.HasCombo;

    IEnumerator BreakCombo(PartyMember partyMember)
    {
        _currentCombo.Break();
        _currentCombo = null;
        BattleEvents.InvokeComboBroken(partyMember);

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator EndCombo()
    {
        _currentCombo.Clear();
        _currentCombo = null;
        BattleEvents.InvokeComboFinished();

        // do some combo feedback animations of damage and bonus damage
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

        _currentActorMarker.Hide();
    }

    void OnBattleAttackReceived(BattleParticipant battleParticipant, BattleAttack battleAttack)
    {
        if (!(battleParticipant is PartyMember) || !battleAttack.IsHit || battleAttack.Damage <= 0 || _currentCombo == null)
            return;

        var attackedPartyMember = battleParticipant as PartyMember;
        if (!_currentCombo.IsParticipant(attackedPartyMember) || _currentCombo.ParticipantsCount < 2)
            return;

        var otherComboParticipants = _currentCombo.Participants.Where(pm => pm != attackedPartyMember).ToList();
        foreach (var partyMember in otherComboParticipants)
        {
            var splashDamageAttack = battleAttack.CreateSplashAttackFor(partyMember);
            StartCoroutine(partyMember.ReceiveAttack(splashDamageAttack));
        }
    }

    void OnDestroy()
    {
        BattleEvents.BattleAttackReceived -= OnBattleAttackReceived;
    }

    void Awake()
    {
        _currentActorMarker = GetComponentInChildren<CurrentActorMarker>();

        BattleEvents.BattleAttackReceived += OnBattleAttackReceived;
    }
}
