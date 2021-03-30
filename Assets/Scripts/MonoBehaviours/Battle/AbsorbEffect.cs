using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorbEffect : MonoBehaviour
{
    [SerializeField] Transform _destinationTransform;

    ParticleSystem _particles;

    public IEnumerator Play(Vector3 sourcePosition, Vector3 destinationPosition)
    {
        transform.position = sourcePosition;
        _destinationTransform.position = destinationPosition;

        var angle = Vector2.SignedAngle(Vector2.up, (destinationPosition - transform.position).normalized);
        _particles.transform.localRotation = Quaternion.Euler(0f, 0f, angle);

        _particles.Play();
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !_particles.isPlaying);

        Destroy(gameObject, 1f);
    }

    void Awake()
    {
        _particles = GetComponentInChildren<ParticleSystem>();    
    }
}
