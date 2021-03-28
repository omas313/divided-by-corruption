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
            yield return ManagePartyMemberTurn(currentBattleParticipant as PartyMember, party, enemies);

        BattleEvents.InvokeBattleParticipantTurnEnded(currentBattleParticipant);

        yield return new WaitForSeconds(_delayBetweenTurns);
    }

    IEnumerator ManageEnemyTurn(Enemy enemy, List<PartyMember> party, List<Enemy> enemies)
    {
        BattleEvents.InvokeEnemyTurnStarted(enemy);

        var battleAction = new BattleAction();
        battleAction.Target = party[UnityEngine.Random.Range(0, party.Count)];

        BattleEvents.InvokeEnemySelectedTarget(battleAction.Target);

        yield return enemy.PerformAction(battleAction, party, enemies);

        BattleEvents.InvokeEnemyTurnEnded(enemy);
    }

    IEnumerator ManagePartyMemberTurn(PartyMember partyMember, List<PartyMember> party, List<Enemy> enemies)
    {
        var currentBattleAction = new BattleAction() { Performer = partyMember };

        partyMember.CharacterStats.IncreaseCurrentMP(1);
        BattleEvents.InvokePartyMemberTurnStarted(partyMember, currentBattleAction);
        BattleUIEvents.InvokeBattleActionTypeSelectionRequested();

        yield return new WaitUntil(() => currentBattleAction.IsValid || Input.GetKeyDown(KeyCode.End));
        if (!currentBattleAction.IsValid)
            yield break;
        
        if (currentBattleAction.Target is Enemy)
            BattleEvents.InvokeEnemyTargetted(currentBattleAction.Target as Enemy);
        
        yield return partyMember.PerformAction(currentBattleAction, party, enemies);

        BattleEvents.InvokePartyMemberTurnEnded(partyMember);
    }
}
