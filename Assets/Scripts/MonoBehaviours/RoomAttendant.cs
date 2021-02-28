using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomAttendant : MonoBehaviour
{
    public Transform GroundLevel => _groundLevel;
    public int RoomNumber =>  _roomNumber;
    public Transform Confiner => _confiner;
    public RoomAttendant PreviousRoomAttendant => _leftDoor != null ? _leftDoor.DestinationRoomAttendant : null;
    public RoomAttendant NextRoomAttendant => _rightDoor != null ? _rightDoor.DestinationRoomAttendant : null;

    [SerializeField] Transform _confiner;
    [SerializeField] Transform _effectsParent;
    [SerializeField] Transform _groundLevel;
    [SerializeField] TeleportingDoor _leftDoor;
    [SerializeField] TeleportingDoor _rightDoor;
    [SerializeField] int _roomNumber;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            EnvironmentEvents.InvokePlayerEnteredRoom(this);
            StartAllEffects();    
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            EnvironmentEvents.InvokePlayerExitedRoom(this);
            StopAllEffects();    
        }
    }

    void StartAllEffects()
    {
        foreach (var particleSystem in _effectsParent.GetComponentsInChildren<ParticleSystem>())
            particleSystem.Play();
    }

    void StopAllEffects()
    {
        foreach (var particleSystem in _effectsParent.GetComponentsInChildren<ParticleSystem>())
            particleSystem.Stop();
    }
}
