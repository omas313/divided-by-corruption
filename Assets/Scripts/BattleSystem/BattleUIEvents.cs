using System;
using System.Collections.Generic;

public class BattleUIEvents
{
    public static event Action BattleActionTypeSelectionRequested;
    public static event Action SpecialAttackSelectionRequested;
    
    public static event Action<PartyMember, List<PartyMember>> PartyMemberTargetSelectionRequested;
    public static event Action EnemyTargetSelectionRequested;
    public static event Action TargetSelectionCancelled;

    public static event Action ActionBarRequested;
    public static event Action ActionBarCompleted;
    public static event Action ComboTrialCompleted;

    public static event Action<BattleParticipant> BattleParticipantHighlighted;
    public static event Action<List<BattleParticipant>> BattleParticipantsHighlighted;


    public static void InvokeBattleActionTypeSelectionRequested() => BattleActionTypeSelectionRequested?.Invoke();
    public static void InvokeSpecialAttackSelectionRequested() => SpecialAttackSelectionRequested?.Invoke();
    
    public static void InvokePartyMemberTargetSelectionRequested(PartyMember performer, List<PartyMember> unselectables) => PartyMemberTargetSelectionRequested?.Invoke(performer, unselectables);
    public static void InvokeEnemyTargetSelectionRequested() => EnemyTargetSelectionRequested?.Invoke();
    public static void InvokeTargetSelectionCancelled() => TargetSelectionCancelled?.Invoke();
    
    public static void InvokeActionBarRequested() => ActionBarRequested?.Invoke();
    public static void InvokeActionBarCompleted() => ActionBarCompleted?.Invoke();
    public static void InvokeComboTrialCompleted() => ComboTrialCompleted?.Invoke();

    internal static void InvokeBattleParticipantHighlighted(BattleParticipant battleParticipant) => BattleParticipantHighlighted?.Invoke(battleParticipant);
    internal static void InvokeBattleParticipantsHighlighted(List<BattleParticipant> battleParticipants) => BattleParticipantsHighlighted?.Invoke(battleParticipants);
}
