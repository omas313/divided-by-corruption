using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboMarkerManager : MonoBehaviour
{
    ComboParticipantMarker[] _markers;

    void HideMarkers()
    {
        foreach (var marker in _markers)
            marker.Hide();
    }

    void ShowMarkersFor(List<PartyMember> participants)
    {
        HideMarkers();

        var i = 0;
        foreach (var participant in participants)
            _markers[i++].PlaceAt(participant.Transform);
    }

    void OnComboParticipantsChanged(List<PartyMember> participants) => ShowMarkersFor(participants);

    void OnComboRequested(List<PartyMember> participants) => ShowMarkersFor(participants);

    void OnComboBroken(PartyMember partyMember) => HideMarkers();
    void OnComboFinished() => HideMarkers();

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

        BattleEvents.ComboRequested += OnComboRequested;
        BattleEvents.ComboParticipantsChanged += OnComboParticipantsChanged;
        BattleEvents.ComboBroken += OnComboBroken;
        BattleEvents.ComboFinished += OnComboFinished;
    }
}
