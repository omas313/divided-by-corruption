using System.Collections;
using UnityEngine;

public class AttackCommand : BattleCommand
{
    public AttackDefinition AttackDefinition { get; private set; }
    public override string Description => $"{Actor.Name} attacks {Target.Name} with {AttackDefinition.Name}";

    public AttackCommand(BattleParticipant actor, AttackDefinition attackDefinition, BattleParticipant target)
        : base(actor, target)
    {
        AttackDefinition = attackDefinition;
    }

    public override IEnumerator Execute()
    {
        yield return Actor.PerformAttack(AttackDefinition, Target);
    }
}
