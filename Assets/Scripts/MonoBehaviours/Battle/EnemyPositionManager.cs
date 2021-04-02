using System.Collections.Generic;

public class EnemyPositionManager : PositionSelectionManager<Enemy>
{
    protected override void OnBattleStarted(List<PartyMember> partyMembers, List<Enemy> enemies)
    {
        InitPositions(enemies);
    }

    void OnEnemyTargetSelectionRequested()
    {
        StartSelection();
    }

    void OnEnemyDied(Enemy enemy)
    {
        RemovePositionOf(enemy);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        BattleEvents.EnemyDied -= OnEnemyDied;
        BattleUIEvents.EnemyTargetSelectionRequested -= OnEnemyTargetSelectionRequested;
    }

    protected override void Awake()
    {
        base.Awake();
        BattleEvents.EnemyDied += OnEnemyDied;
        BattleUIEvents.EnemyTargetSelectionRequested += OnEnemyTargetSelectionRequested;
    }
}
