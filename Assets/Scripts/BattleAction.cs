public class BattleAction
{
    public BattleActionType BattleActionType { get; set; }
    public BattleParticipant Performer { get; set; }
    public BattleParticipant Target { get; set; }
    public AttackBarResult AttackBarResult { get; set; }
    public AttackDefinition AttackDefinition { get; set; }

    public bool IsValid => BattleActionType != BattleActionType.None
        && Performer != null
        && Target != null
        && AttackBarResult != null
        && AttackDefinition != null;
}
