using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboMarkerManager : MonoBehaviour
{
    ComboParticipantMarker[] _markers;

    void HideMarkers()
    {
        _markers[0].Hide();
        _markers[1].Hide();
    }

    void OnComboRequested(PartyMember partyMember1, PartyMember partyMember2)
    {
        _markers[0].PlaceAt(partyMember1.transform);
        _markers[1].PlaceAt(partyMember2.transform);
    }

    void OnComboCancelled(PartyMember partyMember1, PartyMember partyMember2) => HideMarkers();
    void OnComboFinished() => HideMarkers();

    void OnDestroy()
    {
        BattleEvents.ComboRequested -= OnComboRequested;
        BattleEvents.ComboCancelled -= OnComboCancelled;
        BattleEvents.ComboFinished -= OnComboFinished;
    }

    void Awake()
    {
        _markers = GetComponentsInChildren<ComboParticipantMarker>();

        BattleEvents.ComboRequested += OnComboRequested;
        BattleEvents.ComboCancelled += OnComboCancelled;
        BattleEvents.ComboFinished += OnComboFinished;
    }
}
