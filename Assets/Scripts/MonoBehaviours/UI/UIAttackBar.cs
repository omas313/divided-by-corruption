using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAttackBar : MonoBehaviour
{
    [SerializeField] UIAttackBarSegment[] _segments;
    [SerializeField] RectTransform _pin;
    [SerializeField] float _pinSpeed = 20f;
    [SerializeField] GameObject _confirmPointPrefab;
    [SerializeField] Transform _confirmPointsParent;

    RectTransform _rectTransform;
    bool _isMoving;
    float _totalWidth;
    List<float> _currentResult;

    public void StartMovingPin(float pinSpeed)
    {
        StartCoroutine(MovePin(pinSpeed));
    }

    IEnumerator MovePin(float pinSpeed)
    {
        _isMoving = true;

        foreach (var segment in _segments)
            segment.SetActive(true);
        foreach (var confirmPoint in _confirmPointsParent.GetComponentsInChildren<UIConfirmPoint>())
            Destroy(confirmPoint.gameObject);
        _pin.anchoredPosition = Vector3.zero;
        _currentResult = new List<float>();

        while (_pin.anchoredPosition.x < _totalWidth + 5f)
        {
            _pin.anchoredPosition = new Vector3(_pin.anchoredPosition.x + Time.deltaTime * pinSpeed, 0f, 0f);    

            foreach (var segment in _segments)
                if (_pin.anchoredPosition.x > segment.Area.End && segment.IsActive)
                {
                    segment.SetActive(false);
                    _currentResult.Add(0f);
                }

            if (_currentResult.Count == _segments.Length)
                break;

            yield return null;
        }

        foreach (var result in _currentResult)
            Debug.Log(result);
            
        _isMoving = false;
    }

    void Confirm()
    {
        var pinPositionX = _pin.anchoredPosition.x;

        foreach (var segment in _segments)
        {
            if (!segment.IsActive)
                continue;

            if (!segment.IsInside(pinPositionX))
                continue;

            segment.SetActive(false);
            Instantiate(_confirmPointPrefab, _pin.position, Quaternion.identity, _confirmPointsParent);
            _currentResult.Add(segment.GetMultiplier(pinPositionX));
            return;
        }
        
        if (_currentResult.Count < _segments.Length)
        {
            Debug.Log("miss");
            _currentResult.Add(0f);
            Instantiate(_confirmPointPrefab, _pin.position, Quaternion.identity, _confirmPointsParent);
            foreach (var segment in _segments)
                if (segment.IsActive)
                {
                    segment.SetActive(false);
                    break;
                }
        }
    }

    void Update()
    {
        Debug.Log(_isMoving);
        if (Input.GetKeyDown(KeyCode.H) && !_isMoving)
            StartMovingPin(_pinSpeed);

        if (Input.GetKeyDown(KeyCode.G) && _isMoving)
            Confirm();
    }

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _totalWidth = _rectTransform.sizeDelta.x;    
    }
}
