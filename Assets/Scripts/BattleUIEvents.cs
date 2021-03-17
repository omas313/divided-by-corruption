using System;
using System.Collections.Generic;

public class BattleUIEvents
{
    public static event Action BattleActionTypeSelectionRequested;
    public static event Action SpecialAttackSelectionRequested;
    
    public static event Action PartyMemberTargetSelectionRequested;
    public static event Action EnemyTargetSelectionRequested;

    public static event Action RequestedActionBar;

    public static event Action<BattleParticipant> BattleParticipantHighlighted;


    public static void InvokeBattleActionTypeSelectionRequested() => BattleActionTypeSelectionRequested?.Invoke();
    public static void InvokeSpecialAttackSelectionRequested() => SpecialAttackSelectionRequested?.Invoke();
    
    public static void InvokePartyMemberTargetSelectionRequested() => PartyMemberTargetSelectionRequested?.Invoke();
    public static void InvokeEnemyTargetSelectionRequested() => EnemyTargetSelectionRequested?.Invoke();
    
    public static void InvokeRequestedActionBar() => RequestedActionBar?.Invoke();

    internal static void InvokeBattleParticipantHighlighted(BattleParticipant battleParticipant) => BattleParticipantHighlighted?.Invoke(battleParticipant);
}
