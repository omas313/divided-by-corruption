using UnityEngine;
using TMPro;

public class UIBattleMenuItem : UIItem
{
    public BattleActionType BattleActionType => _battleActionType;

    [SerializeField] BattleActionType _battleActionType;

    public void SetActiveState(bool isActive)
    {
        this.isActive = isActive;
        SetState();
    }
}
