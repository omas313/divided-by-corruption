using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComboTrialSegment : MonoBehaviour
{
    public bool IsCorrect { get; private set; }
    public bool IsAnimationDone { get; private set; }

    [SerializeField] GameObject _correctObject;
    [SerializeField] GameObject _wrongObject;
    [SerializeField] Transform _objectParent;
    [SerializeField] Image _overlayImage;

    Animation _overlayAnimation;
    GameObject _currentObject;
    RectTransform _rectTransform;
    Area _area;

    public void SetValue(bool isCorrect)
    {
        _overlayImage.enabled = true;
        IsAnimationDone = false;

        if (IsCorrect == isCorrect && _currentObject != null)
            return;

        if (IsCorrect != isCorrect && _currentObject != null)
            Destroy(_currentObject);

        _currentObject = Instantiate(
            isCorrect ? _correctObject : _wrongObject, 
            _objectParent.position, 
            Quaternion.identity, 
            _objectParent);

        IsCorrect = isCorrect;
    }

    public IEnumerator PlayAnimation()
    {
        IsAnimationDone = false;
        _overlayAnimation.Play();

        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !_overlayAnimation.isPlaying);

        IsAnimationDone = true;
    }

    public bool IsInside(float position) => _area.IsInside(position);

    public void RevealType()
    {
        _overlayImage.enabled = false;
    }

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _overlayAnimation = _overlayImage.GetComponent<Animation>();
        _area = new Area(_rectTransform.anchoredPosition.x, _rectTransform.anchoredPosition.x + _rectTransform.sizeDelta.x);
    }
}
