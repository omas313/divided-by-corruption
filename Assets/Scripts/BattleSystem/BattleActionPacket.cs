using System;
using System.Collections.Generic;
using System.Linq;

public class BattleActionPacket
{
    public BattleAction BattleAction { get; set; }
    public bool HasValidAction => BattleAction != null && BattleAction.IsValid;

    public Combo Combo { get; set; }
    public bool HasCombo => Combo != null;

    public bool HasTargetBeenSet()
    {
        if (!HasCombo)
            return false;

        if (Combo.TargetsCount == 1)
        {
            SetTargets(Combo.Targets.Select(t => t as BattleParticipant).ToList());
            return true;
        }

        return false;
    }

    public bool HasSeveralComboTargets() => HasCombo && Combo.TargetsCount > 1;

    public void ClearCombo() => Combo = null;

    public List<Enemy> GetSelectableTargets() => Combo?.Targets;

    public void SetTargets(BattleParticipant target) => SetTargets(new List<BattleParticipant>() { target });
    public void SetTargets(List<BattleParticipant> targets) => BattleAction.Targets = targets;
}
