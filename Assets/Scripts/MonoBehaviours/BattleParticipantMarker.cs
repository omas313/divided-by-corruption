using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleParticipantMarker : MonoBehaviour
{
    SpriteRenderer[] _spriteRenderers;
    Animation _animation;

    public void PlaceAt(Vector3 position)
    {
        foreach (var renderer in _spriteRenderers)
            renderer.enabled = true;

        _animation.Play();
        transform.position = position + new Vector3(0f, 0.125f, 0f);
    }

    public void Hide()
    {
        foreach (var renderer in _spriteRenderers)
            renderer.enabled = false;

        _animation.Stop();
    }

    public void SetColor(Color color)
    {
        foreach (var renderer in _spriteRenderers)
            renderer.color = color;
    }

    void Awake()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        _animation = GetComponent<Animation>();
        Hide();
    }
}
