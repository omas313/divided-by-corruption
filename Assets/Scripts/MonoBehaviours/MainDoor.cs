using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainDoor : Interactable
{
    [SerializeField] PickupType _pickupType;
    [SerializeField] int _pickupsRequired = 3;
    [SerializeField] ParticleSystem _particlesPrefab;
    [SerializeField] [TextArea(1, 5)] string[] _textLinesOnFailedInteraction;
    [SerializeField] [TextArea(1, 5)] string[] _textLinesOnSuccessfullInteraction;
    
    Animation _doorOpeningAnimation;
    PlayerInventory _inventory;
    int _currentRequiredAmount;
    bool _canInteract = true;
    bool _doorHasOpened;

    protected override void PerformInteraction()
    {
        if (!_canInteract)
            return;

        if (CanOpenDoor())
        {
            StartCoroutine(OpenDoor());
            return;
        }

        if (_doorHasOpened)
            return;

        _canInteract = false;

        if (_inventory == null)
        {
            Debug.Log("Error: Main door doesn't have PlayerInventory");
            return;
        }

        if (!_inventory.HasPickupTypeAmount(_pickupType, amount: 1))
        {
            StartCoroutine(PlayFailedToOpenLines());
            return;
        }

        // dirty, fix later
        EnvironmentEvents.InvokeInteractedWithObject(this, new string[] { _textLinesOnSuccessfullInteraction[_pickupsRequired - _currentRequiredAmount] });
        _currentRequiredAmount--;
        _inventory.Take(_pickupType, amount: 1);
        DestroyOneParticleSystem();

        _canInteract = true;
    }

    void DestroyOneParticleSystem()
    {
        var particles = GetComponentInChildren<ParticleSystem>();
        if (particles == null)
        {
            Debug.Log("Error: trying to destroy particles that don't exists on MainDoor");
            return;
        }
        else
            Destroy(particles.gameObject);
    }

    bool CanOpenDoor() => _currentRequiredAmount <= 0 && !_doorHasOpened;

    IEnumerator PlayFailedToOpenLines()
    {
        EnvironmentEvents.InvokeInteractedWithObject(this, _textLinesOnFailedInteraction);

        yield return new WaitForSeconds(1f);

        _canInteract = true;
    }

    IEnumerator OpenDoor()
    {
        _doorHasOpened = true;
        HideInteractionButton();

        _doorOpeningAnimation.Play();
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !_doorOpeningAnimation.isPlaying);

        EnvironmentEvents.InvokePlayerOpenedMainDoor();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (_doorHasOpened)
            return;

        base.OnTriggerEnter2D(other);

        if (_inventory == null)
            _inventory = other.GetComponent<PlayerInventory>();
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
    }

    protected override void Awake()
    {
        base.Awake();

        _doorOpeningAnimation = GetComponent<Animation>();
        _currentRequiredAmount = _pickupsRequired;

        for (var i = 0; i < _pickupsRequired; i++)
            Instantiate(_particlesPrefab, transform.position, Quaternion.identity, transform);
    }
}
