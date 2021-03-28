public interface IAttackAction
{
    AttackDefinition AttackDefinition { get; set; }
    BattleAttack GetNextBattleAttack();
    bool HasAttacks { get; }
}