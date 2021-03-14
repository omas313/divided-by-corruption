using System.Collections.Generic;

public class BattleAttack
{
    public string Name { get; }
    public int Damage { get; set; }
    public bool IsCritical { get; set; }

    public BattleAttack(AttackDefinition attackDefinition)
    {
        Name = attackDefinition.Name;
        Damage = attackDefinition.Damage;
    }

    public BattleAttack(string name, int damage, bool isCritical)
    {
        Name = name;
        Damage = damage;
        IsCritical = isCritical;
    }
}
