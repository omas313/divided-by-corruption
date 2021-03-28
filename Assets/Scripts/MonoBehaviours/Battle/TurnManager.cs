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

        yield return enemy.GetNextAction(party, enemies).Perform(party, enemies);

        BattleEvents.InvokeEnemyTurnEnded(enemy);
    }

    IEnumerator ManagePartyMemberTurn(PartyMember partyMember, List<PartyMember> party, List<Enemy> enemies)
    {
        var battleActionPacket = new BattleActionPacket();

        BattleEvents.InvokePartyMemberTurnStarted(partyMember, battleActionPacket);
        BattleUIEvents.InvokeBattleActionTypeSelectionRequested();

        yield return new WaitUntil(() => battleActionPacket.HasValidAction || Input.GetKeyDown(KeyCode.End));
        if (!battleActionPacket.HasValidAction)
            yield break;

        if (battleActionPacket.BattleAction.Target is Enemy)
            BattleEvents.InvokeEnemyTargetted(battleActionPacket.BattleAction.Target as Enemy);

        yield return battleActionPacket.BattleAction.Perform(party, enemies);
        
        BattleEvents.InvokePartyMemberTurnEnded(partyMember);
    }
}