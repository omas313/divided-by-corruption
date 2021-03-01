using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{

    [SerializeField] Animation _fadeIn;
    [SerializeField] [TextArea(1, 5)] string[] _lines;
    private bool _isPlayingStory;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Confirm") && !_isPlayingStory)
            StartCoroutine(PlayStory());
    }

    IEnumerator PlayStory()
    {
        _isPlayingStory = true;

        var linesPlayer = FindObjectOfType<UIChronologicalLinesPlayer>();
        linesPlayer.Init(_lines);
        yield return linesPlayer.LinePlayer();
        _fadeIn.Play();
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !_fadeIn.isPlaying);
        GameManager.Instance.LoadRooms();
    }
}
