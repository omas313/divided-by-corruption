using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    BattleActionType _selectedActionType = BattleActionType.None;
    BattleParticipant _selectedTarget;
    AttackBarResult _attackBarResult;

    public IEnumerator Manage(BattleParticipant currentBattleParticipant)
    {
        if (currentBattleParticipant is Enemy)
            yield return ManageEnemyTurn(currentBattleParticipant as Enemy);
        else
            yield return ManagePartyMemberTurn(currentBattleParticipant as PartyMember);
    }

    IEnumerator ManageEnemyTurn(Enemy enemy)
    {
        // BattleEvents.InvokeEnemyTurnStarted(enemy);

        while (true)
        {
            Debug.Log($"{enemy.Name} turn");

            if (Input.GetKeyDown(KeyCode.End))
                break;

            yield return null;
        }

        // BattleEvents.InvokeEnemyTurnEnded(enemy);
    }

    IEnumerator ManagePartyMemberTurn(PartyMember partyMember)
    {
        _attackBarResult = null;
        _selectedTarget = null;
        _selectedActionType = BattleActionType.None;

        BattleEvents.InvokePartyMemberTurnStarted(partyMember);
        yield return new WaitUntil(() => HasValidAction() && HasTarget());

        var segments = partyMember.GetSegmentsFor(_selectedActionType /*,_selectedSkill*/);
        BattleEvents.InvokeRequestedActionBar(segments);
        yield return new WaitUntil(() => _attackBarResult != null);

        // todo: create BattleAction.cs class
        yield return partyMember.PerformAction(_selectedActionType, _attackBarResult, _selectedTarget); 

        BattleEvents.InvokePartyMemberTurnEnded(partyMember);
    }

    bool HasValidAction() => 
        _selectedActionType == BattleActionType.Attack;
        // || (_selectedActionType == ActionType.Skill && _selectedSkill != null);
    
    bool HasTarget() => _selectedTarget != null;

    void OnActionTypeSelected(BattleActionType actionType) => _selectedActionType = actionType;
    void OnTargetSelected(BattleParticipant target) => _selectedTarget = target;
    void OnActionBarCompleted(AttackBarResult attackBarResult) => _attackBarResult = attackBarResult;
    
    void OnDestroy()
    {
        BattleEvents.BattleActionTypeSelected -= OnActionTypeSelected;
        BattleEvents.TargetSelected -= OnTargetSelected;
        BattleEvents.ActionBarCompleted -= OnActionBarCompleted;
    }

    void Awake()
    {
        BattleEvents.BattleActionTypeSelected += OnActionTypeSelected;
        BattleEvents.TargetSelected += OnTargetSelected;
        BattleEvents.ActionBarCompleted += OnActionBarCompleted;
    }
}
