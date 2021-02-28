using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraConfinerPicker : MonoBehaviour
{
    CinemachineConfiner _confiner;

    void OnPlayerEnteredRoom(RoomAttendant roomAttendant)
    {
        _confiner.m_BoundingShape2D = roomAttendant.Confiner.GetComponent<PolygonCollider2D>();
    }

    void Awake()
    {
        _confiner = GetComponent<CinemachineConfiner>();
        EnvironmentEvents.PlayerEnteredRoom += OnPlayerEnteredRoom;
    }

    void OnDestroy()
    {
        EnvironmentEvents.PlayerEnteredRoom -= OnPlayerEnteredRoom;
    }
}
