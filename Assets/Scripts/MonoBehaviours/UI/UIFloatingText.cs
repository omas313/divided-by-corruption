using UnityEngine;
using TMPro;

public class UIFloatingText : MonoBehaviour
{
    public bool IsAvailable => !_animation.isPlaying;

    TextMeshProUGUI _text;
    Animation _animation;

    public void Play(string text, Vector3 position, Color color)
    {
        _text.SetText(text);
        _text.color = color;
        transform.position = position;
        _animation.Play();
    }

    void Awake()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _animation = GetComponent<Animation>();
    }
}
