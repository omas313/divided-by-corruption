using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyMember : BattleParticipant
{
    const string HIT_ANIMATION_BOOL_KEY = "IsGettingHit";
    const string DEATH_ANIMATION_BOOL_KEY = "IsDead";
    const string IDLE_ANIMATION_TRIGGER_KEY = "Idle";
    
    public override string Name => _name;
    public override CharacterStats CharacterStats => _stats;
    public AttackDefinition NormalAttackDefinition => _normalAttackDefinition;
    public DefendDefinition DefendDefinition => _defendDefinition;
    public AbsorbDefinition AbsorbDefinition => _absorbDefinition;
    public List<AttackDefinition> SpecialAttacksDefinitions => _specialAttackDefinitions;

    [SerializeField] string _name;
    [SerializeField] CharacterStats _stats;
    [SerializeField] AttackDefinition _normalAttackDefinition;
    [SerializeField] DefendDefinition _defendDefinition;
    [SerializeField] AbsorbDefinition _absorbDefinition;
    [SerializeField] List<AttackDefinition> _specialAttackDefinitions;

    public override IEnumerator Die()
    {
        Debug.Log($"{Name} Died");
        animator.SetBool(DEATH_ANIMATION_BOOL_KEY, true);
        BattleEvents.InvokePartyMemberDied(this);
        yield return null;
    }

    public override IEnumerator ReceiveAttack(BattleParticipant attacker, BattleAttack attack)
    {
        // Debug.Log($"{Name} ReceiveAttack");
        animator.SetBool(HIT_ANIMATION_BOOL_KEY, true);
        _stats.ReduceCurrentHP(attack.Damage);
        BattleEvents.InvokeHealthDamageReceived(attacker, this, attack);

        yield return new WaitForSeconds(0.5f);
        animator.SetBool(HIT_ANIMATION_BOOL_KEY, false);
    }
}
