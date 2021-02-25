using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyPositionManager : PositionSelectionManager<Enemy>
{
    protected override void OnBattleStarted(List<PartyMember> partyMembers, List<Enemy> enemies)
    {
        InitPositions(enemies);
    }

    protected override void Awake()
    {
        base.Awake();
        BattleEvents.EnemyDied += OnEnemyDied;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        BattleEvents.EnemyDied -= OnEnemyDied;
    }

    void OnEnemyDied(Enemy enemy)
    {
        RemovePositionOf(enemy);
    }
}
