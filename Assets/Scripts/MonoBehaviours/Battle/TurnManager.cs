using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] float _delayBetweenTurns = 2f;

    public IEnumerator Manage(BattleParticipant currentBattleParticipant, List<PartyMember> party, List<Enemy> enemies)
    {
        BattleEvents.InvokeBattleParticipantTurnStarted(currentBattleParticipant);

        if (currentBattleParticipant is Enemy)
            yield return ManageEnemyTurn(currentBattleParticipant as Enemy, party, enemies);
        else
            yield return ManagePartyMemberTurn(currentBattleParticipant as PartyMember);

        BattleEvents.InvokeBattleParticipantTurnEnded(currentBattleParticipant);

        yield return new WaitForSeconds(_delayBetweenTurns);
    }

    IEnumerator ManageEnemyTurn(Enemy enemy, List<PartyMember> party, List<Enemy> enemies)
    {
        BattleEvents.InvokeEnemyTurnStarted(enemy);

        var battleAction = new BattleAction();
        battleAction.Target = party[UnityEngine.Random.Range(0, party.Count)];

        yield return enemy.PerformAction(battleAction); 

        BattleEvents.InvokeEnemyTurnEnded(enemy);
    }

    IEnumerator ManagePartyMemberTurn(PartyMember partyMember)
    {
        var currentBattleAction = new BattleAction() { Attacker = partyMember };

        BattleEvents.InvokePartyMemberTurnStarted(partyMember, currentBattleAction);
        BattleUIEvents.InvokeBattleActionTypeSelectionRequested();

        yield return new WaitUntil(() => currentBattleAction.IsValid);
        
        if (currentBattleAction.Target is Enemy)
            BattleEvents.InvokeEnemyTargetted(currentBattleAction.Target as Enemy);
            
        yield return partyMember.PerformAction(currentBattleAction); 

        BattleEvents.InvokePartyMemberTurnEnded(partyMember);
    }
}
