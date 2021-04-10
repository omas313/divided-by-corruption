using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ComboRequestAction : BattleAction
{
    public override ActionDefinition ActionDefinition => ComboRequestDefinition;
    public override bool IsValid => 
        Performer != null 
        && Targets != null 
        && Targets.Count > 0;
    public ComboRequestDefinition ComboRequestDefinition { get; set; }
    public Combo Combo => _combo;

    Combo _combo;

    public ComboRequestAction(BattleParticipant performer, Combo combo, ComboRequestDefinition comboRequestDefinition)
    {
        BattleActionType = BattleActionType.ComboRequest;
        Performer = performer;
        ComboRequestDefinition = comboRequestDefinition;
        _combo = combo ?? new Combo(performer as PartyMember);
    }

    protected override IEnumerator Perform(List<PartyMember> party, List<Enemy> enemies)
    {
        var targetPartner = Targets[0] as PartyMember;
        var performer = Performer as PartyMember;

        if (_combo.HasParticipants)
            _combo.AddFirstParticipant(targetPartner);
        else
            _combo.AddParticipantsInOrder(targetPartner, performer);

        BattleEvents.InvokeComboRequested(_combo.Participants);
        yield return new WaitForSeconds(0.5f);
    }

}