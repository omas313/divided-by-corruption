using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleCommand
{
    public abstract string Description { get; }

    public BattleParticipant Actor { get; }
    public BattleParticipant Target { get; }

    public BattleCommand(BattleParticipant actor, BattleParticipant target)
    {
        Actor = actor;
        Target = target;
    }

    public abstract IEnumerator Execute();
}
