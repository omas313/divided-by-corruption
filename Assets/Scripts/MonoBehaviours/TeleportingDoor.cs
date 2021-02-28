using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportingDoor : Interactable
{
    [SerializeField] TeleportingDoor _destinationDoor;
    [SerializeField] Transform _groundLevel;

    PlayerController _player;

    public void CatchPlayer(Transform playerTransform)
    {
        playerTransform.position = new Vector2(transform.position.x, _groundLevel.position.y);
    }

    public override void PerformInteraction()
    {
        EnvironmentEvents.FadeInCompleted += OnFadeInCompleted;
        EnvironmentEvents.InvokePlayerTeleporting(); 
    }
    
    void OnFadeInCompleted()
    {
        TeleportPlayer();
        EnvironmentEvents.FadeInCompleted -= OnFadeInCompleted;
    }

    void TeleportPlayer()
    {
        if (_player == null)
            _player = FindObjectOfType<PlayerController>();

        _destinationDoor.CatchPlayer(_player.transform);
        EnvironmentEvents.InvokePlayerTeleported();
    }

    void OnDrawGizmos()
    {
        if (_destinationDoor == null)    
            return;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, _destinationDoor.transform.position);
    }

}
