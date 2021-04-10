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

    void OnPartyMemberDied(PartyMember partyMember)
    {
        _partyBarsMap[partyMember].SetDeathStatus(true);
    }

    void OnPartyMemberTurnStarted(PartyMember partyMember, BattleActionPacket battleActionPacket)
    {
        foreach (var pair in _partyBarsMap)
            _partyBarsMap[pair.Key].SetActiveStatus(pair.Key == partyMember);

        var name = partyMember.Name;
        var hp = partyMember.CharacterStats.CurrentHP.ToString();
        var mp = partyMember.CharacterStats.CurrentMP.ToString();
        _partyBarsMap[partyMember].Init(name, hp, mp, true);

        Show();
    }

    void OnPartyMemberTurnEnded(PartyMember partyMember)
    {
        _partyBarsMap[partyMember].SetActiveStatus(false);
    }
    
    void OnPartyMemberHealthChanged(PartyMember partyMember, int currentValue, int baseValue)
    {
        _partyBarsMap[partyMember].SetHP(currentValue.ToString());
    }

    void OnBattleParticipantsTargetted(List<BattleParticipant> battleParticipants)
    {
        if (battleParticipants[0] is PartyMember)
            Show();
    }
    
    void OnBattleParticipantTurnEnded(BattleParticipant battleParticipant) => Hide();
    void OnActionBarRequested() => Hide();

    void OnDestroy()
    {
        BattleEvents.PartyUpdated -= OnPartyUpdated;
        BattleEvents.PartyMemberDied -= OnPartyMemberDied;
        BattleEvents.PartyMemberTurnStarted -= OnPartyMemberTurnStarted;
        BattleEvents.PartyMemberTurnEnded -= OnPartyMemberTurnEnded;
        BattleUIEvents.ActionBarRequested -= OnActionBarRequested;
        BattleEvents.BattleParticipantsTargetted -= OnBattleParticipantsTargetted;
        BattleEvents.BattleParticipantTurnEnded -= OnBattleParticipantTurnEnded;
        BattleEvents.PartyMemberHealthChanged -= OnPartyMemberHealthChanged;
    }

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        
        BattleEvents.PartyUpdated += OnPartyUpdated;  
        BattleEvents.PartyMemberDied += OnPartyMemberDied;
        BattleEvents.PartyMemberTurnStarted += OnPartyMemberTurnStarted;
        BattleEvents.PartyMemberTurnEnded += OnPartyMemberTurnEnded;
        BattleUIEvents.ActionBarRequested += OnActionBarRequested;
        BattleEvents.BattleParticipantsTargetted += OnBattleParticipantsTargetted;
        BattleEvents.BattleParticipantTurnEnded += OnBattleParticipantTurnEnded;
        BattleEvents.PartyMemberHealthChanged += OnPartyMemberHealthChanged;
    }
}
