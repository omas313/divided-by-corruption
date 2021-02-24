using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEvents
{
    public static event Action<List<PartyMember>, PartyMember> PartyMembersUpdated;
    public static event Action<PartyMember> PartyMemberCommandSet;
    public static event Action<PartyMember> PartyMemberCommandUnset;

    public static void InvokePartyUpdated(List<PartyMember> partyMembers, PartyMember currentActivePartyMember) 
        => PartyMembersUpdated?.Invoke(partyMembers, currentActivePartyMember);
    public static void InvokePartyMemberCommandSet(PartyMember partyMember) => PartyMemberCommandSet?.Invoke(partyMember);
    public static void InvokePartyMemberCommandUnset(PartyMember partyMember) => PartyMemberCommandUnset?.Invoke(partyMember);
}
