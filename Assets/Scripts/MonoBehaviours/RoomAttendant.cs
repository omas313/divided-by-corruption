using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomAttendant : MonoBehaviour
{
    public Transform GroundLevel => _groundLevel;

    [SerializeField] Transform _effectsParent;
    [SerializeField] Transform _groundLevel;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null)
            StartAllEffects();    
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null)
            StopAllEffects();    
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
