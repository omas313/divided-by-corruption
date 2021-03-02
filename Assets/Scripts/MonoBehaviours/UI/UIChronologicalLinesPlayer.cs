using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIChronologicalLinesPlayer : MonoBehaviour
{
    [SerializeField] bool _useDelayBetweenLines = false;
    [SerializeField] float _delayBetweenLines = 2f;
    [SerializeField] float _canvasFadeSpeed = 2f;
    [SerializeField] float _delayToAutoConfirm = 4f;
    [SerializeField] GameEvent _hasFinished;
    [SerializeField] GameObject _cursor;

    CanvasGroup _canvasGroup;
    TextMeshProUGUI _text;
    string[] _lines;

    bool _isPlaying;

    public void Init(string[] lines)
    {
        _lines = lines;
        _cursor.SetActive(false);
    }

    public void PlayLines()
    {
        if (_lines == null || _lines.Length == 0)
        {
            Debug.Log("Error: line player has no lines to play");
            return;
        }
        
        StartCoroutine(LinePlayer());
    }

    public IEnumerator LinePlayer()
    {
        _isPlaying = true;

        _text.SetText("");
        _cursor.SetActive(false);

        yield return FadeInCanvas();

        yield return PlayLinesWithDelay();
        _cursor.SetActive(true);

        yield return WaitForConfirmationOrTimeout();

        _cursor.SetActive(false);
        _hasFinished.Raise();

        yield return FadeOutCanvas();

        _isPlaying = false;
    }

    IEnumerator FadeInCanvas()
    {
        while (_canvasGroup.alpha < 1f)
        {
            _canvasGroup.alpha += Time.deltaTime * _canvasFadeSpeed;
            yield return null;
        }
    }

    IEnumerator FadeOutCanvas()
    {
        while (_canvasGroup.alpha > 0f)
        {
            _canvasGroup.alpha -= Time.deltaTime * _canvasFadeSpeed;
            yield return null;
        }
    }
    
    IEnumerator PlayLinesWithDelay()
    {
        _cursor.SetActive(true);

        for (int i = 0; i < _lines.Length; i++)
        {
            _text.SetText(_lines[i].Trim());

            if (_useDelayBetweenLines)
                yield return new WaitForSeconds(_delayBetweenLines);
            else
            {
                yield return new WaitUntil(() => Input.GetButtonDown("Confirm"));
                yield return new WaitForSeconds(0.15f);
            }
        }
    }

    IEnumerator WaitForConfirmationOrTimeout()
    {
        var timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;

            if (Input.GetButtonDown("Confirm") || timer > _delayToAutoConfirm)
                break;

            yield return null;
        }
    }
    
    void OnInteractedWithObject(Interactable interactable, string[] lines)
    {
        if (_isPlaying)
        {
            StopAllCoroutines();
            _canvasGroup.alpha = 0f;
        }

        _lines = lines;
        StartCoroutine(LinePlayer());
    }

    void Awake()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _canvasGroup.alpha = 0f;

        EnvironmentEvents.InteractedWithObject += OnInteractedWithObject;
    }

    void OnDestroy()
    {
        EnvironmentEvents.InteractedWithObject -= OnInteractedWithObject;
    }
}
