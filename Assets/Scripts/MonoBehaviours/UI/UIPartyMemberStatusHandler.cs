using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPartyMemberStatusHandler : MonoBehaviour
{
    [SerializeField] UIPartyMemberStatusBar _barPrefab;
    [SerializeField] RectTransform _barsParent;
    
    UIPartyMemberStatusBar[] _bars;
    Dictionary<PartyMember, UIPartyMemberStatusBar> _partyBarsMap;
    CanvasGroup _canvasGroup;
    
    public void Hide() => _canvasGroup.alpha = 0f;

    public void Show() => _canvasGroup.alpha = 1f;


    void Init(List<PartyMember> party)
    {
        _partyBarsMap = new Dictionary<PartyMember, UIPartyMemberStatusBar>();

        _bars = new UIPartyMemberStatusBar[party.Count];
        for (var i = 0; i < party.Count; i++)
        {
            var bar = Instantiate(_barPrefab, _barsParent.position, Quaternion.identity, _barsParent);
            _bars[i] = bar;
            _partyBarsMap[party[i]] = bar;
        }
    }

    void OnPartyUpdated(List<PartyMember> party)
    {
        if (_partyBarsMap == null)
            Init(party);

        foreach (var pair in _partyBarsMap)
        {
            var partyMember = pair.Key;
            var bar = pair.Value;

            var name = partyMember.Name;
            var hp = partyMember.CharacterStats.CurrentHP.ToString();
            var mp = partyMember.CharacterStats.CurrentMP.ToString();
            
            bar.Init(name, hp, mp);
        }
    }

    void OnCurrentPartyMemberChanged(PartyMember partyMember)
    {
        foreach (var pair in _partyBarsMap)
            _partyBarsMap[pair.Key].SetActiveStatus(pair.Key == partyMember);
    }
    
    void OnPartyMemberDied(PartyMember partyMember)
    {
        _partyBarsMap[partyMember].SetDeathStatus(true);
    }

    void OnActionBarRequested() => Hide();
    void OnPartyMemberTurnStarted(PartyMember partyMember, BattleAction battleAction)
    {
        var name = partyMember.Name;
        var hp = partyMember.CharacterStats.CurrentHP.ToString();
        var mp = partyMember.CharacterStats.CurrentMP.ToString();
        _partyBarsMap[partyMember].Init(name, hp, mp, true);

        Show();
    }

    void OnDestroy()
    {
        BattleEvents.PartyUpdated -= OnPartyUpdated;
        BattleEvents.CurrentPartyMemberChanged -= OnCurrentPartyMemberChanged;
        BattleEvents.PartyMemberDied -= OnPartyMemberDied;
        BattleEvents.PartyMemberTurnStarted -= OnPartyMemberTurnStarted;
        BattleUIEvents.ActionBarRequested -= OnActionBarRequested;
    }

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        
        BattleEvents.PartyUpdated += OnPartyUpdated;  
        BattleEvents.CurrentPartyMemberChanged += OnCurrentPartyMemberChanged;
        BattleEvents.PartyMemberDied += OnPartyMemberDied;
        BattleEvents.PartyMemberTurnStarted += OnPartyMemberTurnStarted;
        BattleUIEvents.ActionBarRequested += OnActionBarRequested;
    }
}
