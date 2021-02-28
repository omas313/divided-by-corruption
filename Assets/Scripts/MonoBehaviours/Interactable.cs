using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class Interactable : MonoBehaviour
{
    // public string Text => _text;
    
    protected bool isInteracting;

    [SerializeField] CanvasGroup _canvas;
    [SerializeField] bool _canUse = true;
    [SerializeField] [TextArea(1, 5)] string[] _cantUseTextLines;
    
    Collider2D _collider;
    Animation _interactionAnimation;
    bool _isPlayerDetected;

    public abstract void PerformInteraction();

    void OnTriggerEnter2D(Collider2D other)
    {
        _isPlayerDetected = true;
        ShowInteractionButton();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        _isPlayerDetected = false;
        HideInteractionButton();
    }

    void ShowInteractionButton()
    {
        _canvas.alpha = 1f;
        _interactionAnimation.Play();
    }

    void HideInteractionButton()
    {
        _canvas.alpha = 0f;
        _interactionAnimation.Stop();
    }

    void Interact()
    {
        HideInteractionButton();
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

        _collider = GetComponent<Collider2D>();
        _collider.isTrigger = true;

        HideInteractionButton();
    }
}
