using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScene : MonoBehaviour
{
    [SerializeField] Animation _fadeIn;

    void Start()
    {
       StartCoroutine(LoadMainMenuAfterConfirmation()); 
    }

    IEnumerator LoadMainMenuAfterConfirmation()
    {
        yield return new WaitUntil(() => Input.GetButtonDown("Confirm"));
        
        _fadeIn.Play();
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !_fadeIn.isPlaying);

        GameManager.Instance.LoadMainMenu();
    }

}
