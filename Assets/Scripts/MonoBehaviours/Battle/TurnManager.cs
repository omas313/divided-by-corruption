using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    BattleAction _currentBattleAction;

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
        _currentBattleAction = new BattleAction() { Attacker = partyMember };

        BattleEvents.InvokePartyMemberTurnStarted(partyMember, _currentBattleAction);
        BattleUIEvents.InvokeBattleActionTypeSelectionRequested();

        yield return new WaitUntil(() => _currentBattleAction.IsValid);
        yield return partyMember.PerformAction(_currentBattleAction); 

        BattleEvents.InvokePartyMemberTurnEnded(partyMember);
        _currentBattleAction = null;
    }
}
