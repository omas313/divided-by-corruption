using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UISpecialAttackMenuItem : UIItem
{
    public AttackDefinition AttackDefinition => _attackDefinition;

    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] TextMeshProUGUI _mpCost;
    
    AttackDefinition _attackDefinition;

    public void Init(AttackDefinition attackDefinition)
    {
        _attackDefinition = attackDefinition;

        _text.SetText(attackDefinition.Name);
        // todo: set mp cost later
        // _mpCost.SetText(attackDefinition.MPCost);
    }

    public void SetActiveState(bool isActive)
    {
        this.isActive = isActive;
        SetState();
    }
}
