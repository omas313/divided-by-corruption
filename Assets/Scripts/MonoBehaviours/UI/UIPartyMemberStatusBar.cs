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


    public void Init(string name, string hp, string mp, bool isActive = false)
    {
        _nameText.SetText(name);
        _hpText.SetText(hp);
        _mpText.SetText(mp);
        this.isActive = isActive;

        SetState();
    }
    
    public void SetCommandStatus(bool hasCommand)
    {
        Debug.Log($"setting {gameObject.name} to {hasCommand}");
        _commandSetText.gameObject.SetActive(hasCommand);
    }

    protected override void Awake()
    {
        base.Awake();
        SetCommandStatus(false);
    }
}
