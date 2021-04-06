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
    public static event Action<PartyMember, int, int> PartyMemberHealthChanged;
    public static event Action<PartyMember, BattleActionPacket> PartyMemberTurnStarted;
    public static event Action<PartyMember> PartyMemberTurnEnded;
    public static event Action<PartyMember> PartyMemberDied;
    public static event Action<PartyMember, PartyMember> ComboRequested;
    public static event Action<PartyMember, PartyMember> ComboCancelled;
    public static event Action<BattleActionPacket> ComboTrialRequested;
    public static event Action ComboFinished;
    public static event Action<BattleParticipant, string> ComboEffectsGained;



    public static event Action<BattleParticipant> CriticalAttackReceived;
    public static event Action<BattleParticipant> MissedAttackReceived;

    public static event Action<List<BattleParticipant>> BattleParticipantsTargetted;

    public static event Action<Enemy> EnemyTurnStarted;
    public static event Action<Enemy> EnemyTurnEnded;
    public static event Action<Enemy> EnemyDied;
    public static event Action<List<BattleParticipant>> EnemySelectedTargets;
    public static event Action<Enemy, int, int> EnemyHealthChanged;
    public static event Action<Enemy, int, int> EnemyArmourChanged;


    public static event Action<BattleParticipant, int, bool> HealthDamageReceived;
    public static event Action<BattleParticipant, int, bool> ArmourDamageReceived;
    public static event Action<BattleParticipant> ArmourBreak;
    public static event Action<BattleParticipant, BattleParticipant, int> MPAbsorbed;


    public static event Action<bool> BattleEnded;


    public static void InvokeBattleStarted(List<PartyMember> partyMembers, List<Enemy> enemies) => BattleStarted?.Invoke(partyMembers, enemies);
    public static void InvokeBattleParticipantsUpdated(List<BattleParticipant> battleParticipants) => BattleParticipantsUpdated?.Invoke(battleParticipants);
    public static void InvokeBattleParticipantTurnStarted(BattleParticipant battleParticipant) => BattleParticipantTurnStarted?.Invoke(battleParticipant);
    public static void InvokeBattleParticipantTurnEnded(BattleParticipant battleParticipant) => BattleParticipantTurnEnded?.Invoke(battleParticipant);
    public static void InvokeBattleParticipantDied(BattleParticipant battleParticipant) => BattleParticipantDied?.Invoke(battleParticipant);

    public static void InvokePartyUpdated(List<PartyMember> partyMembers) => PartyUpdated?.Invoke(partyMembers);
    public static void InvokeCurrentPartyMemberChanged(PartyMember currentPartyMember) => CurrentPartyMemberChanged?.Invoke(currentPartyMember);
    public static void InvokePartyMemberTurnStarted(PartyMember partyMember, BattleActionPacket battleActionPacket) => PartyMemberTurnStarted?.Invoke(partyMember, battleActionPacket);
    public static void InvokePartyMemberHealthChanged(PartyMember partyMember, int currentValue, int baseValue) => PartyMemberHealthChanged?.Invoke(partyMember, currentValue, baseValue);
    public static void InvokePartyMemberTurnEnded(PartyMember partyMember) => PartyMemberTurnEnded?.Invoke(partyMember);
    public static void InvokePartyMemberDied(PartyMember partyMember) => PartyMemberDied?.Invoke(partyMember);
    public static void InvokeComboRequested(PartyMember partyMember1, PartyMember partyMember2) => ComboRequested?.Invoke(partyMember1, partyMember2);
    public static void InvokeComboCancelled(PartyMember partyMember1, PartyMember partyMember2) => ComboCancelled?.Invoke(partyMember1, partyMember2);
    public static void InvokeComboTrialRequested(BattleActionPacket battleActionPacket) => ComboTrialRequested?.Invoke(battleActionPacket);
    public static void InvokeComboFinished() => ComboFinished?.Invoke();
    public static void InvokeComboEffectsGained(BattleParticipant battleParticipant, string effectsString) => ComboEffectsGained?.Invoke(battleParticipant, effectsString);

    public static void InvokeMissedAttackReceived(BattleParticipant target) => MissedAttackReceived?.Invoke(target);
    public static void InvokeBattleParticipantsTargetted(List<BattleParticipant> battleParticipants) => BattleParticipantsTargetted?.Invoke(battleParticipants);

    public static void InvokeEnemyTurnStarted(Enemy enemy) => EnemyTurnStarted?.Invoke(enemy);
    public static void InvokeEnemyTurnEnded(Enemy enemy) => EnemyTurnEnded?.Invoke(enemy);
    public static void InvokeEnemyDied(Enemy enemy) => EnemyDied?.Invoke(enemy);
    public static void InvokeEnemySelectedTargets(List<BattleParticipant> battleParticipants) => EnemySelectedTargets?.Invoke(battleParticipants);
    public static void InvokeEnemyHealthChanged(Enemy enemy, int currentValue, int baseValue) => EnemyHealthChanged?.Invoke(enemy, currentValue, baseValue);
    public static void InvokeEnemyArmourChanged(Enemy enemy, int currentValue, int baseValue) => EnemyArmourChanged?.Invoke(enemy, currentValue, baseValue);


    public static void InvokeHealthDamageReceived(BattleParticipant receiver, int damage, bool isCritical) => HealthDamageReceived?.Invoke(receiver, damage, isCritical);
    public static void InvokeArmourDamageReceived(BattleParticipant receiver, int damage, bool isCritical) => ArmourDamageReceived?.Invoke(receiver, damage, isCritical);
    public static void InvokeArmourBreak(BattleParticipant battleParticipant) => ArmourBreak?.Invoke(battleParticipant);
    public static void InvokeMPAbsorbed(BattleParticipant performer, BattleParticipant target, int amount) => MPAbsorbed?.Invoke(performer, target, amount);


    public static void InvokeBattleEnded(bool hasWon) => BattleEnded?.Invoke(hasWon);
}
