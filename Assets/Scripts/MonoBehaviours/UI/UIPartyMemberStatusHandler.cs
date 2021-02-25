using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPartyMemberStatusHandler : MonoBehaviour
{
    [SerializeField] UIPartyMemberStatusBar _barPrefab;
    [SerializeField] RectTransform _barsParent;
    
    UIPartyMemberStatusBar[] _bars;
    Dictionary<PartyMember, UIPartyMemberStatusBar> _partyBars;
    
    void OnPartyUpdated(List<PartyMember> party, PartyMember currentActiveMember)
    {
        if (_partyBars == null)
            Init(party);

        foreach (var pair in _partyBars)
        {
            var partyMember = pair.Key;
            var bar = pair.Value;

            var name = partyMember.Name;
            var hp = partyMember.CharacterStats.CurrentHP.ToString();
            var mp = partyMember.CharacterStats.CurrentMP.ToString();
            
            bar.Init(name, hp, mp, isActive: currentActiveMember == partyMember);
        }
    }

    void Init(List<PartyMember> party)
    {
        _partyBars = new Dictionary<PartyMember, UIPartyMemberStatusBar>();

        _bars = new UIPartyMemberStatusBar[party.Count];
        for (var i = 0; i < party.Count; i++)
        {
            var bar = Instantiate(_barPrefab, _barsParent.position, Quaternion.identity, _barsParent);
            _bars[i] = bar;
            _partyBars[party[i]] = bar;
        }
    }
    
    void OnPartyMemberCommandUnset(PartyMember partyMember)
    {
        _partyBars[partyMember].SetCommandStatus(false);
    }

    void OnPartyMemberCommandSet(PartyMember partyMember)
    {
        _partyBars[partyMember].SetCommandStatus(true);
    }

    void OnPartyMemberDied(PartyMember partyMember)
    {
        _partyBars[partyMember].SetDeathStatus(true);
    }

    void Awake()
    {
        BattleEvents.PartyMembersUpdated += OnPartyUpdated;       
        BattleEvents.PartyMemberCommandSet += OnPartyMemberCommandSet;
        BattleEvents.PartyMemberCommandUnset += OnPartyMemberCommandUnset;
        BattleEvents.PartyMemberDied += OnPartyMemberDied;
    }

    void OnDestroy()
    {
        BattleEvents.PartyMembersUpdated -= OnPartyUpdated;        
        BattleEvents.PartyMemberCommandSet -= OnPartyMemberCommandSet;
        BattleEvents.PartyMemberCommandUnset -= OnPartyMemberCommandUnset;
        BattleEvents.PartyMemberDied -= OnPartyMemberDied;
    }
}
