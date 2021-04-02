using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentActorMarker : MonoBehaviour
{
    SpriteRenderer _spriteRenderer;
    Animation _animation;
    Transform _originalParent;

    public void Mark(Transform targetTransform)
    {
        _spriteRenderer.enabled = true;
        _animation.Play();

        transform.parent = targetTransform;
        transform.localPosition = Vector3.zero;
    }

    public void Hide()
    {
        _spriteRenderer.enabled = false;
        _animation.Stop();

        transform.parent = _originalParent;
    }

    void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animation = GetComponent<Animation>();
        _originalParent = transform.parent;

        Hide();
    }
}
