using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] float _delayBetweenTurns = 2f;
    CurrentActorMarker _marker;

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

        _marker.Mark(enemy.TopMarkerTransform);
        yield return new WaitForSeconds(1f);
        _marker.Hide();

        yield return enemy.GetNextAction(party, enemies).PerformAction(party, enemies);

        enemy.EndTurn();
        BattleEvents.InvokeEnemyTurnEnded(enemy);
    }

    IEnumerator ManagePartyMemberTurn(PartyMember partyMember, List<PartyMember> party, List<Enemy> enemies, Enemy target = null) 
    {
        var battleActionPacket = new BattleActionPacket(target);

        BattleEvents.InvokePartyMemberTurnStarted(partyMember, battleActionPacket);
        _marker.Mark(partyMember.TopMarkerTransform);
        BattleUIEvents.InvokeBattleActionTypeSelectionRequested();

        yield return new WaitUntil(() => battleActionPacket.HasValidAction || Input.GetKeyDown(KeyCode.End));
        // end button hack to make sure turn ends since packet is invalid
        if (!battleActionPacket.HasValidAction)
            yield break;
        
        _marker.Hide();
        yield return battleActionPacket.BattleAction.PerformAction(party, enemies);

        partyMember.EndTurn();
        BattleEvents.InvokePartyMemberTurnEnded(partyMember);
        
        yield return new WaitForSeconds(1f);

        if (ShouldCombo(partyMember, battleActionPacket))
            yield return StartComboTrial(partyMember, battleActionPacket, party, enemies);
    }

    bool ShouldCombo(PartyMember partyMember, BattleActionPacket battleActionPacket) => 
        partyMember.HasComboPartner 
        && (battleActionPacket.BattleAction.BattleActionType == BattleActionType.Attack
        || battleActionPacket.BattleAction.BattleActionType == BattleActionType.Special);

    IEnumerator StartComboTrial(PartyMember firstAttacker, BattleActionPacket firstAttackPacket, List<PartyMember> party, List<Enemy> enemies)
    {
        _marker.Mark(firstAttacker.TopMarkerTransform);

        var battleActionPacket = new BattleActionPacket();
        var comboTrialAction = new ComboTrialAction(firstAttacker, firstAttacker.ComboPartner, firstAttackPacket.BattleAction.ActionDefinition as AttackDefinition);
        battleActionPacket.BattleAction = comboTrialAction;

        var singleTargetList = new List<BattleParticipant>() { firstAttackPacket.BattleAction.Targets[0] };
        BattleEvents.InvokeBattleParticipantsTargetted(singleTargetList);
        BattleEvents.InvokeComboTrialRequested(battleActionPacket);

        yield return new WaitUntil(() => battleActionPacket.HasValidAction || Input.GetKeyDown(KeyCode.End));
        if (!battleActionPacket.HasValidAction)
            yield break;

        yield return battleActionPacket.BattleAction.PerformAction(party, enemies);

        if (comboTrialAction.IsSuccess)
        {
            yield return ManagePartyMemberTurn(firstAttacker.ComboPartner, party, enemies, firstAttackPacket.BattleAction.Targets[0] as Enemy);
            firstAttacker.RemoveComboPartner();
            BattleEvents.InvokeComboFinished();
        }
        else
            _marker.Hide();
    }

    void Awake()
    {
        _marker = GetComponentInChildren<CurrentActorMarker>();
    }
}