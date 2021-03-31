public interface IAttackAction
{
    AttackDefinition AttackDefinition { get; set; }
    bool HasAttacks { get; }
}