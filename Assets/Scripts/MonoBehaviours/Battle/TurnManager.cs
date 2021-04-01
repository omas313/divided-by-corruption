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
        yield return enemy.GetNextAction(party, enemies).PerformAction(party, enemies);
        enemy.EndTurn();
        BattleEvents.InvokeEnemyTurnEnded(enemy);
    }

    IEnumerator ManagePartyMemberTurn(PartyMember partyMember, List<PartyMember> party, List<Enemy> enemies, Enemy target = null) 
    {
        var battleActionPacket = new BattleActionPacket(target);

        BattleEvents.InvokePartyMemberTurnStarted(partyMember, battleActionPacket);
        BattleUIEvents.InvokeBattleActionTypeSelectionRequested();

        yield return new WaitUntil(() => battleActionPacket.HasValidAction || Input.GetKeyDown(KeyCode.End));
        // end button hack
        if (!battleActionPacket.HasValidAction)
            yield break;

        yield return battleActionPacket.BattleAction.PerformAction(party, enemies);

        partyMember.EndTurn();
        BattleEvents.InvokePartyMemberTurnEnded(partyMember);

        if (ShouldCombo(partyMember, battleActionPacket))
            yield return StartCombo(partyMember, battleActionPacket, party, enemies);
    }

    bool ShouldCombo(PartyMember partyMember, BattleActionPacket battleActionPacket) => 
        partyMember.HasComboPartner 
        && (battleActionPacket.BattleAction.BattleActionType == BattleActionType.Attack
        || battleActionPacket.BattleAction.BattleActionType == BattleActionType.Special);

    IEnumerator StartCombo(PartyMember firstAttacker, BattleActionPacket firstAttackPacket, List<PartyMember> party, List<Enemy> enemies)
    {
        var battleActionPacket = new BattleActionPacket();
        var comboTrialAction = new ComboTrialAction(firstAttacker, firstAttacker.ComboPartner, firstAttackPacket.BattleAction.ActionDefinition as AttackDefinition);
        battleActionPacket.BattleAction = comboTrialAction;

        BattleEvents.InvokeComboTrialRequested(battleActionPacket);        
        Debug.Log("requested trial, waiting for result, press end for success");
        yield return new WaitUntil(() => battleActionPacket.HasValidAction || Input.GetKeyDown(KeyCode.End) || Input.GetKeyDown(KeyCode.Home));
        comboTrialAction.ForceSuccess();
        // if (!battleActionPacket.HasValidAction)
        //     yield break;

        yield return battleActionPacket.BattleAction.PerformAction(party, enemies);

        if (comboTrialAction.IsSuccess)
        {
            yield return ManagePartyMemberTurn(firstAttacker.ComboPartner, party, enemies, firstAttackPacket.BattleAction.Targets[0] as Enemy);
            firstAttacker.RemoveComboPartner();
            BattleEvents.InvokeComboFinished();
        }
    }
}