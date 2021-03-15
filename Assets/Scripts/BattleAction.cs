public class BattleAction
{
    public BattleActionType BattleActionType { get; set; }
    public PartyMember Attacker { get; set; }
    public BattleParticipant Target { get; set; }
    public AttackBarResult AttackBarResult { get; set; }
    public AttackDefinition AttackDefinition { get; set; }

    public bool IsValid => BattleActionType != BattleActionType.None
        && Attacker != null
        && Target != null
        && AttackBarResult != null
        && AttackDefinition != null;
}
