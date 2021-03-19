using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInitiatorEnemy : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 2f;
    [SerializeField] float _stoppingDistance = 1.5f;
    [SerializeField] BattleDataDefinition _battleDefinition;

    Transform _target;
    bool _isMoving;

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
        if (_target != null && !_isMoving)
            StartCoroutine(BeginBattleInitiation());           
    }

    IEnumerator BeginBattleInitiation()
    {
        _isMoving = true;

        EnvironmentEvents.InvokeBattleInitiated(_battleDefinition);

        if (FindObjectOfType<PlayerController>().transform.position.x > transform.position.x)
            GetComponentInChildren<SpriteRenderer>().flipX = true;

        while (Vector2.Distance(transform.position, _target.position) > _stoppingDistance)
        {
            transform.Translate((_target.transform.position - transform.position) * Time.deltaTime * _moveSpeed);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _stoppingDistance);    
    }
}
