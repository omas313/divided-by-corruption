using UnityEngine;
using TMPro;

public class UIBattleMenuItem : UIItem
{
    [SerializeField] GameEvent _event;

    public void SetActiveState(bool isActive)
    {
        this.isActive = isActive;
        SetState();
    }
    
    protected override void Awake()
    {
        base.Awake();
    }
}
