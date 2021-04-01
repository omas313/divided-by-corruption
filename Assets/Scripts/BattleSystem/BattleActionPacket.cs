public class BattleActionPacket
{
    public BattleAction BattleAction { get; set; }
    public bool HasValidAction => BattleAction != null && BattleAction.IsValid;
    public Enemy ComboTarget { get; set; }
    public bool HasComboTarget => ComboTarget != null;

    public BattleActionPacket(Enemy target = null)
    {
        ComboTarget = target;
    }
}