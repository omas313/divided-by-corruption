using System;
using System.Collections;
using UnityEngine;

public class TurnManager : MonoBehaviour
{

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
        // BattleEvents.InvokePartyMemberTurnStarted(partyMember);

        while (true)
        {
            Debug.Log($"{partyMember.Name} turn");

            if (Input.GetKeyDown(KeyCode.End))
                break;
            
            yield return null;
        }

        // BattleEvents.InvokePartyMemberTurnEnded(partyMember);
    }


    void Awake()
    {
            
    }

}
