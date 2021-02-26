using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingStuff : MonoBehaviour
{
    [SerializeField] Transform _toMove;
    [SerializeField] Transform _target;
    private float startTime;
    private float journeyLength;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoveToPosition(_target.position));   
    }

    
    IEnumerator MoveToPosition(Vector3 destination)
    {
        startTime = Time.time;

        journeyLength = Vector2.Distance(_toMove.position, destination);

        while (Vector2.Distance(destination, _toMove.position) > 0.1f)
        {
            Debug.Log("in loop");
            float distance = (Time.time - startTime) * 0.1f;
            float distancePercentage = distance / journeyLength;

            _toMove.position = Vector3.Lerp(_toMove.position, destination, distancePercentage);

            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
