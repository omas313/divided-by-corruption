using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : Interactable
{
    // Maybe SO for ItemDefinition later
    public string Name => _name;
    public Sprite Sprite => _sprite;
    public PickupType Type => _type;

    [SerializeField] PickupType _type;
    [SerializeField] string _name;
    [SerializeField] Sprite _sprite;
    [SerializeField] bool _playTextLinesOnPickup = false;
    [SerializeField] [TextArea(1, 5)] string[] _textLinesOnPickup;

    protected override void PerformInteraction()
    {
        colliderComponent.enabled = false;
        spriteRenderer.enabled = false;
        EnvironmentEvents.InvokePlayerPickedUpObject(_type);

        if (_playTextLinesOnPickup)
            EnvironmentEvents.InvokeInteractedWithObject(this, _textLinesOnPickup);
    }

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer.sprite = _sprite;
    }
}