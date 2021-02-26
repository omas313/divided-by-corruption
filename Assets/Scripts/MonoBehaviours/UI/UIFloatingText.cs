using UnityEngine;
using TMPro;

public class UIFloatingText : MonoBehaviour
{
    public bool IsAvailable => !_animation.isPlaying;

    [SerializeField] TextMeshProUGUI _damageText;
    [SerializeField] TextMeshProUGUI _additionalText;

    Animation _animation;

    public void Play(string text, Vector3 position, Color colour, string additionalText = "")
    {
        _damageText.SetText(text);
        _damageText.color = colour;

        _additionalText.SetText(additionalText);
        _additionalText.color = colour;

        transform.position = position;
        _animation.Play();
    }

    void Awake()
    {
        _animation = GetComponent<Animation>();
    }
}
