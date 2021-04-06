using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyMemebersPositionManager : PositionSelectionManager<PartyMember>
{
    protected override void OnBattleStarted(List<PartyMember> partyMembers, List<Enemy> enemies)
    {
        InitPositions(partyMembers);
    }

    void OnPartyMemberDied(PartyMember partyMember)
    {
        RemovePositionOf(partyMember);
    }
    void OnPartyMemberTargetSelectionRequested(PartyMember performer, List<PartyMember> unselectables)
    {
        this.unselectables = unselectables;
        StartSelection();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        BattleEvents.PartyMemberDied -= OnPartyMemberDied;
        BattleUIEvents.PartyMemberTargetSelectionRequested -= OnPartyMemberTargetSelectionRequested;
    }

    protected override void Awake()
    {
        base.Awake();
        BattleEvents.PartyMemberDied += OnPartyMemberDied;
        BattleUIEvents.PartyMemberTargetSelectionRequested += OnPartyMemberTargetSelectionRequested;
    }
}
