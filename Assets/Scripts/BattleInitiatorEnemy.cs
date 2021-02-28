using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInitiatorEnemy : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 2f;

    Transform _target;

    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();

        if (player != null && _target == null)
            _target = player.transform;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();

        if (player != null)
            _target = null;
    }

    void Update()
    {
        if (_target != null)
            MoveTowardsTarget();    
    }

    void MoveTowardsTarget()
    {
        transform.Translate((_target.transform.position - transform.position) * Time.deltaTime * _moveSpeed);
    }
}
