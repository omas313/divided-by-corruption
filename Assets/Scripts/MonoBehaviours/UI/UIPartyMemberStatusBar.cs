using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UIPartyMemberStatusBar : UIItem
{
    [SerializeField] TextMeshProUGUI _commandSetText;
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _hpText;
    [SerializeField] TextMeshProUGUI _mpText;
    [SerializeField] GameObject _koText;
    [SerializeField] GameObject _stats;


    public void Init(string name, string hp, string mp, bool isActive = false)
    {
        _nameText.SetText(name);
        _hpText.SetText(hp);
        _mpText.SetText(mp);

        SetActiveStatus(isActive);
    }

    public void SetActiveStatus(bool isActive)
    {
        this.isActive = isActive;
        SetState();
    }
    
    public void SetDeathStatus(bool isDead)
    {
        SetActiveStatus(!isDead);
        _koText.SetActive(isDead);
        _stats.SetActive(!isDead);
    }

    protected override void Awake()
    {
        base.Awake();
        SetActiveStatus(false);
        SetDeathStatus(false);
    }

    public void SetHP(string currentValue)
    {
        _hpText.SetText(currentValue);
    }
}
