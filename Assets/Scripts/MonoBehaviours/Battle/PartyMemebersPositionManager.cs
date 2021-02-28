using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyMemebersPositionManager : PositionSelectionManager<PartyMember>
{
    protected override void OnBattleStarted(List<PartyMember> partyMembers, List<Enemy> enemies)
    {
        InitPositions(partyMembers);
    }

    protected override void Awake()
    {
        base.Awake();
        BattleEvents.PartyMemberDied += OnPartyMemberDied;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        BattleEvents.PartyMemberDied -= OnPartyMemberDied;
    }

    void OnPartyMemberDied(PartyMember partyMember)
    {
        RemovePositionOf(partyMember);
    }
}
