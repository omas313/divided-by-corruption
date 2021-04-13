using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ComboDisplayManager : MonoBehaviour
{
    ComboParticipantMarker[] _markers;
    ComboConnectorLine[] _lines;

    void HideLines()
    {
        foreach (var line in _lines)
            line.Hide();
    }

    void HideMarkers()
    {
        foreach (var marker in _markers)
            marker.Hide();
    }

    void ShowLinesFor(List<PartyMember> participants)
    {
        HideLines();

        foreach (var participant1 in participants)
        {
            foreach (var participant2 in participants)
            {
                if (participant1 == participant2 || _lines.Any(l => l.IsActive && l.PartyMembers.Contains(participant1) && l.PartyMembers.Contains(participant2)))
                    continue;

                var inactiveLine = _lines.FirstOrDefault(l => !l.IsActive);
                inactiveLine?.SetBetweenPartyMembers(participant1, participant2);
            }
        }

    }

    void ShowMarkersFor(List<PartyMember> participants)
    {
        HideMarkers();

        var i = 0;
        foreach (var participant in participants)
            _markers[i++].PlaceAt(participant.Transform);
    }

    void OnComboParticipantsChanged(List<PartyMember> participants)
    {
        ShowMarkersFor(participants);
        ShowLinesFor(participants);
    }

    void OnComboRequested(List<PartyMember> participants)
    {
        ShowMarkersFor(participants);
        ShowLinesFor(participants);
    }

    void OnComboBroken(PartyMember partyMember)
    {
        HideMarkers();
        HideLines();
    }

    void OnComboFinished()
    {
        HideMarkers();
        HideLines();
    }

    void OnDestroy()
    {
        BattleEvents.ComboRequested -= OnComboRequested;
        BattleEvents.ComboParticipantsChanged -= OnComboParticipantsChanged;
        BattleEvents.ComboBroken -= OnComboBroken;
        BattleEvents.ComboFinished -= OnComboFinished;
    }

    void Awake()
    {
        _markers = GetComponentsInChildren<ComboParticipantMarker>();
        _lines = GetComponentsInChildren<ComboConnectorLine>();

        BattleEvents.ComboRequested += OnComboRequested;
        BattleEvents.ComboParticipantsChanged += OnComboParticipantsChanged;
        BattleEvents.ComboBroken += OnComboBroken;
        BattleEvents.ComboFinished += OnComboFinished;
    }
}
