using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMarker : MonoBehaviour
{
    SpriteRenderer[] _spriteRenderers;

    public void PlaceAt(Vector3 position)
    {
        foreach (var renderer in _spriteRenderers)
            renderer.enabled = true;

        transform.position = position;
    }

    public void Hide()
    {
        foreach (var renderer in _spriteRenderers)
            renderer.enabled = false;
    }

    public void SetColor(Color color)
    {
        foreach (var renderer in _spriteRenderers)
            renderer.color = color;
    }

    void Awake()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        Hide();
    }
}
