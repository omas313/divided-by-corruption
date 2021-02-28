using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const string WALK_ANIMATION_BOOL_KEY = "IsWalking";

    [SerializeField] float _moveSpeed;
    [SerializeField] Transform _spriteTransform;

    bool IsWalking => _rigidbody.velocity.magnitude > Mathf.Epsilon;
    Animator _animator;
    Rigidbody2D _rigidbody;
    float _xThrow;
    Vector3 _scale;
    

    void Update()
    {
        _xThrow = Input.GetAxis("Horizontal");
    }
    void FixedUpdate()
    {
        _rigidbody.velocity = new Vector2(_xThrow * _moveSpeed, 0f);
        _animator.SetBool(WALK_ANIMATION_BOOL_KEY, IsWalking);

        _scale = _spriteTransform.localScale;
        if (_xThrow > 0)
            _scale.x = 1;
        else if (_xThrow < 0)
            _scale.x = -1;

        _spriteTransform.localScale = _scale;
    }

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }
}
