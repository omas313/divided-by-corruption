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

    public void RaiseEvent()
    {
        if (_event != null)
            _event.Raise();
    }
}
