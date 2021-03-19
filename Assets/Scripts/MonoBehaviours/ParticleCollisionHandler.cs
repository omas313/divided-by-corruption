using System;
using UnityEngine;

public class ParticleCollisionHandler : MonoBehaviour
{
    public event Action Collided;

    void OnParticleCollision(GameObject other)
    {
        Collided?.Invoke();
    }
}
