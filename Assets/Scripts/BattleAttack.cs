public class BattleAttack
{
    public string Name { get; }
    public int Damage { get; set; }
    public AttackType AttackType { get; set; }

    public BattleAttack(AttackDefinition attackDefinition)
    {
        Name = attackDefinition.Name;
        Damage = attackDefinition.Damage;
        AttackType = attackDefinition.AttackType;
    }
}