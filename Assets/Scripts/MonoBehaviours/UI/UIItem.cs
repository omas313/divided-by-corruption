using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class UIItem : MonoBehaviour
{
    protected bool isActive = false;

    [SerializeField] Image _overlayImage;
    [SerializeField] Animation _animation;
    [SerializeField] Image _bg;
    [SerializeField] TextMeshProUGUI[] _texts;

    readonly Vector3 _inactiveScale = new Vector3(0.85f, 1f, 1f);
    Color _overlayImageActiveColor;
    Color _overlayImageInactiveColor;

    public void SetSelected()
    {
        _animation.Stop();
    }

    protected void SetState()
    {
        _overlayImage.color = isActive ? _overlayImageActiveColor : _overlayImageInactiveColor;
        transform.localScale = isActive ? Vector3.one : _inactiveScale;

        HandleAnimation();
    }

    void HandleAnimation()
    {
        if (_animation == null)
            return;

        if (isActive)
            _animation.Play();
        else
            _animation.Stop();
    }

    protected virtual void Start()
    {
        _bg.color = Theme.Instance.PrimaryLighterColor;

        foreach (var text in _texts)
            text.color = Theme.Instance.TextColor;
    }

    protected virtual void Awake()
    {
        _overlayImageActiveColor = _overlayImage.color;    
        _overlayImageInactiveColor = new Color(_overlayImage.color.r, _overlayImage.color.g, _overlayImage.color.b, 0.5f);    
    }
}
