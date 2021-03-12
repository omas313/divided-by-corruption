using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBarPin : MonoBehaviour
{
    public bool IsInNormalArea { get; private set; }
    public bool IsInCriticalArea { get; private set; }

    SpriteRenderer _spriteRenderer;
    Rigidbody2D _rigidbody;


    public void Activate(Vector3 position, float speed)
    {
        _spriteRenderer.enabled = true;
        transform.position = position;
        _rigidbody.velocity = new Vector2(speed, 0f);
    }

    public void Deactivate()
    {
        _rigidbody.velocity = Vector2.zero;
        _spriteRenderer.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        IsInNormalArea = collider.CompareTag("NormalHitArea");
        IsInCriticalArea = collider.CompareTag("CriticalHitArea");
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("NormalHitArea"))
            IsInNormalArea = false;
        else if (collider.CompareTag("CriticalHitArea"))
            IsInCriticalArea = false;
    }

    void Update()
    {
        Debug.Log($"IsInNormalArea: {IsInNormalArea}");
        Debug.Log($"IsInCriticalArea: {IsInCriticalArea}");
    }

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }
}
