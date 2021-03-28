public class BattleActionPacket
{
    public BattleAction BattleAction { get; set; }
    public bool HasValidAction => BattleAction != null && BattleAction.IsValid;
}