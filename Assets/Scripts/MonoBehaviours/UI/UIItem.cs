using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIItem : MonoBehaviour
{
    protected bool isActive = false;

    [SerializeField] Image _overlayImage;
    [SerializeField] Animation _animation;

    readonly Vector3 _inactiveScale = new Vector3(0.85f, 0.85f, 0.85f);
    Color _overlayImageActiveColor;
    Color _overlayImageInactiveColor;

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

    protected virtual void Awake()
    {
        _overlayImageActiveColor = _overlayImage.color;    
        _overlayImageInactiveColor = new Color(_overlayImage.color.r, _overlayImage.color.g, _overlayImage.color.b, 0.75f);    
    }
}
