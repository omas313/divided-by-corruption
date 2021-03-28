using System.Collections;
using System.Collections.Generic;

public class DefendAction : BattleAction
{
    public override bool IsValid => Performer != null && Target != null;

    public DefendAction(BattleParticipant performer)
    {
        BattleActionType = BattleActionType.Defend;
        Performer = Target = performer;
    }

    public override IEnumerator Perform(List<PartyMember> party, List<Enemy> enemies)
    {
        throw new System.NotImplementedException();
    }
}
