using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UISpecialAttackMenuItem : UIItem
{
    public AttackDefinition AttackDefinition => _attackDefinition;
    public bool IsSelectable { get; private set; }

    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] TextMeshProUGUI _mpCost;
    
    readonly Color _darkGreyColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    AttackDefinition _attackDefinition;

    public void Init(AttackDefinition attackDefinition, bool isSelectable)
    {
        _attackDefinition = attackDefinition;

        _text.SetText(attackDefinition.Name);

        if (attackDefinition.MPCost == 0)
            _mpCost.gameObject.SetActive(false);
        else
        {
            _mpCost.gameObject.SetActive(true);
            _mpCost.SetText(attackDefinition.MPCost.ToString());
        }

        IsSelectable = isSelectable;

        if (!IsSelectable)
            _mpCost.color = _darkGreyColor;
    }

    public void SetActiveState(bool isActive)
    {
        this.isActive = isActive;
        SetState();
    }
}
