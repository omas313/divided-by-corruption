using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendAction : BattleAction
{
    public override ActionDefinition ActionDefinition => DefendDefinition;
    public override bool IsValid => Performer != null && Targets != null && Targets.Count > 0;

    public DefendDefinition DefendDefinition { get; set; }

    public DefendAction(BattleParticipant performer)
    {
        BattleActionType = BattleActionType.Defend;
        Performer = performer;
        Targets.Add(performer);
    }

    protected override IEnumerator Perform(List<PartyMember> party, List<Enemy> enemies)
    {
        var effect = new Effect(DefendDefinition.DefendEffect);
        effect.SetDuration(DefendDefinition.Duration + 1);

        Performer.EffectsManager.AddEffect(effect);
        yield return new WaitForSeconds(1f);
    }
}
