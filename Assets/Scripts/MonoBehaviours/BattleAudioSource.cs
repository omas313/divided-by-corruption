using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAudioSource : MonoBehaviour
{
    public static BattleAudioSource Instance { get; private set; }

    [SerializeField] AudioClip _selectClip;
    [SerializeField] AudioClip _unselectClip;

    AudioSource _audioSource;

    public void PlaySelectSound()
    {
        _audioSource.PlayOneShot(_selectClip);
    }

    public void PlayUnselectSound()
    {
        _audioSource.PlayOneShot(_unselectClip);
    }

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

}
