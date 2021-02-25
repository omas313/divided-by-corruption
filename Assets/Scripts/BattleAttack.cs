public class BattleAttack
{
    public string Name { get; }
    public int Damage { get; set; }
    public DamageType DamageType { get; set; }

    public BattleAttack(AttackDefinition attackDefinition)
    {
        Name = attackDefinition.Name;
        Damage = attackDefinition.Damage;
        DamageType = attackDefinition.DamageType;
    }
}