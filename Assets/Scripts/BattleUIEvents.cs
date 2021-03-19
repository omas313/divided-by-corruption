using System;
using System.Collections.Generic;

public class BattleUIEvents
{
    public static event Action BattleActionTypeSelectionRequested;
    public static event Action SpecialAttackSelectionRequested;
    
    public static event Action PartyMemberTargetSelectionRequested;
    public static event Action EnemyTargetSelectionRequested;
    public static event Action TargetSelectionCancelled;

    public static event Action ActionBarRequested;
    public static event Action ActionBarCompleted;

    public static event Action<BattleParticipant> BattleParticipantHighlighted;


    public static void InvokeBattleActionTypeSelectionRequested() => BattleActionTypeSelectionRequested?.Invoke();
    public static void InvokeSpecialAttackSelectionRequested() => SpecialAttackSelectionRequested?.Invoke();
    
    public static void InvokePartyMemberTargetSelectionRequested() => PartyMemberTargetSelectionRequested?.Invoke();
    public static void InvokeEnemyTargetSelectionRequested() => EnemyTargetSelectionRequested?.Invoke();
    public static void InvokeTargetSelectionCancelled() => TargetSelectionCancelled?.Invoke();
    
    public static void InvokeActionBarRequested() => ActionBarRequested?.Invoke();
    public static void InvokeActionBarCompleted() => ActionBarCompleted?.Invoke();

    internal static void InvokeBattleParticipantHighlighted(BattleParticipant battleParticipant) => BattleParticipantHighlighted?.Invoke(battleParticipant);
}
