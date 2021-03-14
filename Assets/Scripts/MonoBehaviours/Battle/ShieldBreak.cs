using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBreak : MonoBehaviour
{
    public void Play()
    {
        GetComponent<Animation>().Play();
        // Debug.Log("playing shield break");
    }
}
