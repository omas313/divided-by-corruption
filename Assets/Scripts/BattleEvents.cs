using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEvents
{
    public static event Action<List<PartyMember>, List<Enemy>> BattleStarted;
    public static event Action<List<BattleParticipant>> BattleParticipantsUpdated;
    public static event Action<BattleParticipant> BattleParticipantTurnStarted;
    public static event Action<BattleParticipant> BattleParticipantTurnEnded;
    public static event Action<BattleParticipant> BattleParticipantDied;

    
    public static event Action<List<PartyMember>> PartyUpdated;
    public static event Action<PartyMember> CurrentPartyMemberChanged;
    public static event Action<PartyMember, BattleAction> PartyMemberTurnStarted;
    public static event Action<PartyMember> PartyMemberTurnEnded;
    public static event Action<PartyMember> PartyMemberDied;

    public static event Action<Vector3> AttackCritAt;
    public static event Action<Vector3> AttackMissedAt;

    public static event Action<Enemy> EnemyTurnStarted;
    public static event Action<Enemy> EnemyTurnEnded;
    public static event Action<Enemy> EnemyDied;
    public static event Action<Enemy> EnemyTargetted;
    public static event Action<BattleParticipant> EnemySelectedTarget;
    public static event Action<Enemy, int, int> EnemyHealthChanged;
    public static event Action<Enemy, int, int> EnemyArmourChanged;

    public static event Action<BattleParticipant, BattleParticipant, BattleAttack> HealthDamageReceived;
    public static event Action<BattleParticipant, BattleParticipant, BattleAttack> ArmourDamageReceived;
    public static event Action<BattleParticipant> ArmourBreak;

    public static event Action<bool> BattleEnded;

    public static void InvokeBattleStarted(List<PartyMember> partyMembers, List<Enemy> enemies) => BattleStarted?.Invoke(partyMembers, enemies);
    public static void InvokeBattleParticipantsUpdated(List<BattleParticipant> battleParticipants) => BattleParticipantsUpdated?.Invoke(battleParticipants);
    public static void InvokeBattleParticipantTurnStarted(BattleParticipant battleParticipant) => BattleParticipantTurnStarted?.Invoke(battleParticipant);
    public static void InvokeBattleParticipantTurnEnded(BattleParticipant battleParticipant) => BattleParticipantTurnEnded?.Invoke(battleParticipant);
    public static void InvokeBattleParticipantDied(BattleParticipant battleParticipant) => BattleParticipantDied?.Invoke(battleParticipant);

    public static void InvokePartyUpdated(List<PartyMember> partyMembers) => PartyUpdated?.Invoke(partyMembers);
    public static void InvokeCurrentPartyMemberChanged(PartyMember currentPartyMember) => CurrentPartyMemberChanged?.Invoke(currentPartyMember);
    public static void InvokePartyMemberTurnStarted(PartyMember partyMember, BattleAction battleAction) => PartyMemberTurnStarted?.Invoke(partyMember, battleAction);
    public static void InvokePartyMemberTurnEnded(PartyMember partyMember) => PartyMemberTurnEnded?.Invoke(partyMember);
    public static void InvokePartyMemberDied(PartyMember partyMember) => PartyMemberDied?.Invoke(partyMember);

    public static void InvokeAttackCritAt(Vector3 position) => AttackCritAt?.Invoke(position);
    public static void InvokeAttackMissAt(Vector3 position) => AttackMissedAt?.Invoke(position);


    public static void InvokeEnemyTurnStarted(Enemy enemy) => EnemyTurnStarted?.Invoke(enemy);
    public static void InvokeEnemyTurnEnded(Enemy enemy) => EnemyTurnEnded?.Invoke(enemy);
    public static void InvokeEnemyDied(Enemy enemy) => EnemyDied?.Invoke(enemy);
    public static void InvokeEnemyTargetted(Enemy enemy) => EnemyTargetted?.Invoke(enemy);
    public static void InvokeEnemySelectedTarget(BattleParticipant battleParticipant) => EnemySelectedTarget?.Invoke(battleParticipant);
    public static void InvokeEnemyHealthChanged(Enemy enemy, int currentValue, int baseValue) => EnemyHealthChanged?.Invoke(enemy, currentValue, baseValue);
    public static void InvokeEnemyArmourChanged(Enemy enemy, int currentValue, int baseValue) => EnemyArmourChanged?.Invoke(enemy, currentValue, baseValue);

    public static void InvokeHealthDamageReceived(BattleParticipant attacker, BattleParticipant defender, BattleAttack attack) 
        => HealthDamageReceived?.Invoke(attacker, defender, attack);
    public static void InvokeArmourDamageReceived(BattleParticipant attacker, BattleParticipant defender, BattleAttack attack) 
        => ArmourDamageReceived?.Invoke(attacker, defender, attack);
    public static void InvokeArmourBreak(BattleParticipant battleParticipant) => ArmourBreak?.Invoke(battleParticipant);

    public static void InvokeBattleEnded(bool hasWon) => BattleEnded?.Invoke(hasWon);
}
