using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BattleUIAttackGameEvent : GameEvent
{
    public override void Raise()
    {
        base.Raise();

        BattleEvents.InvokeAttackSelected();
    }
}
