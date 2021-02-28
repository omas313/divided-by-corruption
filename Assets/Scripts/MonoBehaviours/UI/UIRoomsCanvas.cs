using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRoomsCanvas : MonoBehaviour
{
    [SerializeField] Animation _fadeIn;
    [SerializeField] Animation _fadeOut;

    void OnPlayerTeleported()
    {
        StartCoroutine(FadeOut());
    }

    void OnPlayerTeleporting()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        _fadeOut.gameObject.SetActive(false);
        _fadeIn.gameObject.SetActive(true);
        
        _fadeIn.Play();

        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !_fadeIn.isPlaying);

        _fadeIn.gameObject.SetActive(false);
        EnvironmentEvents.InvokeFadeInCompleted();
    }

    IEnumerator FadeOut()
    {
        _fadeOut.gameObject.SetActive(true);
        _fadeIn.gameObject.SetActive(false);

        _fadeOut.Play();

        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !_fadeOut.isPlaying);

        _fadeOut.gameObject.SetActive(false);
        EnvironmentEvents.InvokeFadeOutCompleted();
    }

    void Awake()
    {
        EnvironmentEvents.PlayerTeleporting += OnPlayerTeleporting;
        EnvironmentEvents.PlayerTeleported += OnPlayerTeleported;
    }

    void OnDestroy()
    {
        EnvironmentEvents.PlayerTeleporting -= OnPlayerTeleporting;
        EnvironmentEvents.PlayerTeleported -= OnPlayerTeleported;
    }
}
