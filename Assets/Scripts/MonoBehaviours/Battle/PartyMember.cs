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
    public ComboRequestDefinition ComboRequestDefinition => _comboRequestDefinition;
    public ComboTrialDefinition ComboTrialDefinition => _comboTrialDefinition;
    public List<AttackDefinition> SpecialAttacksDefinitions => _specialAttackDefinitions;
    public PartyMember ComboPartner => _comboPartner;
    public bool HasComboPartner => _comboPartner != null;

    [SerializeField] string _name;
    [SerializeField] CharacterStats _stats;
    [SerializeField] AttackDefinition _normalAttackDefinition;
    [SerializeField] DefendDefinition _defendDefinition;
    [SerializeField] AbsorbDefinition _absorbDefinition;
    [SerializeField] ComboRequestDefinition _comboRequestDefinition;
    [SerializeField] ComboTrialDefinition _comboTrialDefinition;
    [SerializeField] List<AttackDefinition> _specialAttackDefinitions;

    PartyMember _comboPartner;

    public void SetComboPartner(PartyMember partyMember) => _comboPartner = partyMember;
    public void RemoveComboPartner() => _comboPartner = null;

    public override IEnumerator Die()
    {
        Debug.Log($"{Name} Died");
        animator.SetBool(DEATH_ANIMATION_BOOL_KEY, true);
        BattleEvents.InvokePartyMemberDied(this);
        yield return null;
    }

    public override IEnumerator ReceiveAttack(BattleParticipant attacker, BattleAttack attack)
    {
        animator.SetBool(HIT_ANIMATION_BOOL_KEY, true);

        attack.Damage = CharacterStats.ApplyDefenseModifier(attack.Damage);
        TakeDamage(attacker, attack);
        
        yield return new WaitForSeconds(0.25f);
        animator.SetBool(HIT_ANIMATION_BOOL_KEY, false);
    }
}
