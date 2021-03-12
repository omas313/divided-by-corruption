using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEvents
{
    public static event Action<List<PartyMember>, List<Enemy>> BattleStarted;
    public static event Action<List<PartyMember>, List<Enemy>> BattleParticipantsUpdated;
    
    public static event Action<List<PartyMember>, PartyMember> PartyUpdated;
    public static event Action<PartyMember> CurrentPartyMemberChanged;
    public static event Action<PartyMember> PartyMemberDied;

    public static event Action<BattleParticipant> TargetSelected;
    public static event Action AttackSelected;

    // public static event Action<SkillDefition> SkillSelected;

    public static event Action<Enemy> EnemyDied;
    public static event Action<Enemy, int, int> EnemyHealthChanged;
    public static event Action<Enemy, int, int> EnemyArmourChanged;

    public static event Action<BattleParticipant, BattleParticipant, BattleAttack> HealthDamageReceived;
    public static event Action<BattleParticipant, BattleParticipant, BattleAttack> ArmourDamageReceived;
    public static event Action<BattleParticipant> ArmourBreak;

    public static event Action<bool> BattleEnded;

    public static void InvokeBattleStarted(List<PartyMember> partyMembers, List<Enemy> enemies) => BattleStarted?.Invoke(partyMembers, enemies);
    public static void InvokeBattleParticipantsUpdated(List<PartyMember> partyMembers, List<Enemy> enemies) => BattleParticipantsUpdated?.Invoke(partyMembers, enemies);

    public static void InvokePartyUpdated(List<PartyMember> partyMembers, PartyMember currentActivePartyMember) 
        => PartyUpdated?.Invoke(partyMembers, currentActivePartyMember);
    public static void InvokeCurrentPartyMemberChanged(PartyMember currentPartyMember) => CurrentPartyMemberChanged?.Invoke(currentPartyMember);
    public static void InvokePartyMemberDied(PartyMember partyMember) => PartyMemberDied?.Invoke(partyMember);

    public static void InvokeTargetSelected(BattleParticipant participant) => TargetSelected?.Invoke(participant);
    public static void InvokeAttackSelected() => AttackSelected?.Invoke();
    // public static void InvokeSkillSelected(SkillDefinition skillDefinition) => SkillSelected?.Invoke(skillDefinition);

    public static void InvokeEnemyDied(Enemy enemy) => EnemyDied?.Invoke(enemy);
    public static void InvokeEnemyHealthChanged(Enemy enemy, int currentValue, int baseValue) => EnemyHealthChanged?.Invoke(enemy, currentValue, baseValue);
    public static void InvokeEnemyArmourChanged(Enemy enemy, int currentValue, int baseValue) => EnemyArmourChanged?.Invoke(enemy, currentValue, baseValue);

    public static void InvokeHealthDamageReceived(BattleParticipant attacker, BattleParticipant defender, BattleAttack attack) 
        => HealthDamageReceived?.Invoke(attacker, defender, attack);
    public static void InvokeArmourDamageReceived(BattleParticipant attacker, BattleParticipant defender, BattleAttack attack) 
        => ArmourDamageReceived?.Invoke(attacker, defender, attack);
    public static void InvokeArmourBreak(BattleParticipant battleParticipant) => ArmourBreak?.Invoke(battleParticipant);

    public static void InvokeBattleEnded(bool hasWon) => BattleEnded?.Invoke(hasWon);
}
