using System;
using System.Collections.Generic;

public class BattleAttack
{
    public string Name { get; set; }
    public int Damage { get; set; }
    public int ActualDamageTaken { get; set; }
    public bool IsHit { get; set; }
    public bool IsCritical { get; set; }
    public BattleParticipant Attacker { get; set; }
    public List<BattleParticipant> Targets { get; set; }
    public AttackDefinition AttackDefinition { get; set; }
    public bool IsSplashAttack { get; set; }

    public BattleAttack() {}

    public BattleAttack CreateSplashAttackFor(PartyMember partyMember) => new BattleAttack()
        {
            Attacker = Attacker,
            Targets = new List<BattleParticipant>() { partyMember },
            Name = Name,
            Damage = (int)Math.Ceiling(Damage * AttackDefinition.SplashDamageModifier),
            IsHit = IsHit,
            IsCritical = IsCritical,
            AttackDefinition = AttackDefinition,
            IsSplashAttack = true
        };
}
