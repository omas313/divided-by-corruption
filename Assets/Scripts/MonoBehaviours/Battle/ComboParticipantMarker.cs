using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboParticipantMarker : MonoBehaviour
{
    SpriteRenderer _spriteRenderer;
    Transform _originalParent;
    Animation _animation;

    public void Show()
    {
        _animation.Play();
        _spriteRenderer.enabled = true;
    }
        
    public void Hide()
    {
        _animation.Stop();
        _spriteRenderer.enabled = false;
        transform.parent = _originalParent;
    }

    public void PlaceAt(Transform partyMemberTransform)
    {
        transform.parent = partyMemberTransform;
        transform.localPosition = Vector3.zero;
        Show();
    }

    void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animation = GetComponent<Animation>();
        _originalParent = transform.parent;
    }
}
