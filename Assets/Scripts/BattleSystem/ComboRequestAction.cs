using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboRequestAction : BattleAction
{
    public override ActionDefinition ActionDefinition => ComboRequestDefinition;
    public override bool IsValid => 
        Performer != null 
        && Targets != null 
        && Targets.Count > 0;

    public ComboRequestDefinition ComboRequestDefinition { get; set; }

    public ComboRequestAction(BattleParticipant performer)
    {
        BattleActionType = BattleActionType.ComboRequest;
        Performer = performer;
    }

    protected override IEnumerator Perform(List<PartyMember> party, List<Enemy> enemies)
    {
        var target = Targets[0] as PartyMember;
        target.SetComboPartner(Performer as PartyMember);
        BattleEvents.InvokeComboRequested(Performer as PartyMember, target);
        yield return new WaitForSeconds(1f);
    }
}