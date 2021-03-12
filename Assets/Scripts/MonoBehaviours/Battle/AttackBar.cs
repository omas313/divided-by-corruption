using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBar : MonoBehaviour
{
    [SerializeField] AttackBarPin _pin;
    [SerializeField] float _pinSpeed = 1f;
    [SerializeField] Transform _start;
    [SerializeField] Transform _end;

    bool _isActive;

    public void MovePin()
    {
        StartCoroutine(MovePinCoroutine());
    }

    IEnumerator MovePinCoroutine()
    {
        _isActive = true;
        _pin.Activate(_start.position, _pinSpeed);

        yield return new WaitUntil(() => _pin.transform.position.x >= _end.transform.position.x);

        _pin.Deactivate();
        _isActive = false;
    }

    void Confirm()
    {
        Debug.Log("confirmed");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && !_isActive)
            MovePin();    

        if (Input.GetKeyDown(KeyCode.G) && _isActive)
            Confirm();    
    }

    void Awake()
    {
        // MovePin();
    }
}
