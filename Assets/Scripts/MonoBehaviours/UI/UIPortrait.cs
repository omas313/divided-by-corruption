using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPortrait : MonoBehaviour
{
    [SerializeField] Image _highlightImage;
    [SerializeField] Image _bg;
    [SerializeField] RectTransform _bgImageRectTransform;
    [SerializeField] Image _specialImage;
    [SerializeField] Image _portraitImage;
    
    BattleParticipant _battleParticipant;

    public void Init(BattleParticipant battleParticipant)
    {
        _battleParticipant = battleParticipant;
        _portraitImage.sprite = _battleParticipant.PortraitSprite;
        SetHighlightedState(false);
    }

    public bool IsPortraitFor(BattleParticipant battleParticipant) => _battleParticipant == battleParticipant;

    public void SetHighlightedState(bool isHighlighted) => _highlightImage.enabled = isHighlighted;

    void Start()
    {
        _bg.color = Theme.Instance.PrimaryLighterColor;

        if (_specialImage != null)
            _specialImage.color = Theme.Instance.ContrastColor;
    }
}
