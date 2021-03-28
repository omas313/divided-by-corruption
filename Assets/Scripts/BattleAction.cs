using System.Collections;
using System.Collections.Generic;

public abstract class BattleAction
{
    public BattleActionType BattleActionType { get; set; }
    public BattleParticipant Performer { get; set; }
    public BattleParticipant Target { get; set; }
    public abstract bool IsValid { get; }

    public abstract IEnumerator Perform(List<PartyMember> party, List<Enemy> enemies);
}
