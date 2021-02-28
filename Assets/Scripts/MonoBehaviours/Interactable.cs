using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Interactable : MonoBehaviour
{
    protected bool isInteracting;
    protected Collider2D colliderComponent;
    protected SpriteRenderer spriteRenderer;

    [SerializeField] CanvasGroup _canvas;
    [SerializeField] bool _canUse = true;
    [SerializeField] [TextArea(1, 5)] string[] _cantUseTextLines;
    
    Animation _interactionAnimation;
    bool _isPlayerDetected;

    protected virtual void PerformInteraction() {}

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        _isPlayerDetected = true;
        ShowInteractionButton();
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        _isPlayerDetected = false;
        HideInteractionButton();
    }

    protected void ShowInteractionButton()
    {
        _canvas.alpha = 1f;
        _interactionAnimation.Play();
    }

    protected void HideInteractionButton()
    {
        _canvas.alpha = 0f;
        _interactionAnimation.Stop();
    }

    void Interact()
    {
        isInteracting = true;

        if (!_canUse)
            EnvironmentEvents.InvokeInteractedWithObject(this, _cantUseTextLines);
        else
            PerformInteraction();
    }

    void OnCompletedInteractionWithObject(Interactable interactable)
    {
        if (interactable == this)
            isInteracting = false;
    }

    void Update()
    {
        if (!_isPlayerDetected) 
            return;

        if (Input.GetButtonDown("Up"))
            Interact();
    }

    protected virtual void Awake()
    {
        _interactionAnimation = _canvas.GetComponentInChildren<Animation>();

        colliderComponent = GetComponent<Collider2D>();
        colliderComponent.isTrigger = true;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        HideInteractionButton();
    }
}
