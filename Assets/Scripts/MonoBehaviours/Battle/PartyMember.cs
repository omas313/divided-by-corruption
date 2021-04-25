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
    public override BattleModifiers BattleModifiers => _battleModifiers;
    public AttackDefinition NormalAttackDefinition => _normalAttackDefinition;
    public DefendDefinition DefendDefinition => _defendDefinition;
    public AbsorbDefinition AbsorbDefinition => _absorbDefinition;
    public ComboRequestDefinition ComboRequestDefinition => _comboRequestDefinition;
    public ComboTrialDefinition ComboTrialDefinition => _comboTrialDefinition;
    public List<AttackDefinition> SpecialAttacksDefinitions => _specialAttackDefinitions;

    [SerializeField] string _name;
    [SerializeField] CharacterStats _stats;
    [SerializeField] BattleModifiers _battleModifiers;
    [SerializeField] AttackDefinition _normalAttackDefinition;
    [SerializeField] DefendDefinition _defendDefinition;
    [SerializeField] AbsorbDefinition _absorbDefinition;
    [SerializeField] ComboRequestDefinition _comboRequestDefinition;
    [SerializeField] ComboTrialDefinition _comboTrialDefinition;
    [SerializeField] List<AttackDefinition> _specialAttackDefinitions;
    [SerializeField] List<EffectDefinition> _effectDefinitions;

    public override IEnumerator Die()
    {
        Debug.Log($"{Name} Died");
        animator.SetBool(DEATH_ANIMATION_BOOL_KEY, true);
        BattleEvents.InvokePartyMemberDied(this);
        yield return null;
    }

    public override IEnumerator ReceiveAttack(BattleAttack attack)
    {
        if (!attack.IsHit)
        {
            BattleEvents.InvokeMissedAttackReceived(this);
            yield break;
        }

        animator.SetBool(HIT_ANIMATION_BOOL_KEY, true);

        var damageToTake = BattleModifiers.ApplyDefenseModifier(attack.Damage);
        damageToTake = attack.Attacker.BattleModifiers.ApplyHealthDamageModifier(damageToTake);
        attack.ActualDamageTaken = damageToTake;
        
        CharacterStats.ReduceCurrentHP(damageToTake);
        BattleEvents.InvokePartyMemberHealthChanged(this, _stats.CurrentHP, _stats.BaseHP);
        BattleEvents.InvokeHealthDamageReceived(this, damageToTake, attack.IsCritical);
        
        if (!attack.IsSplashAttack)
            BattleEvents.InvokeBattleAttackReceived(this, attack);
        
        yield return new WaitForSeconds(0.25f);
        animator.SetBool(HIT_ANIMATION_BOOL_KEY, false);
    }

    public bool CanLearnEffectModifier(EffectDefinition effectDefinition)
    {
        return !_effectDefinitions.Any(emd => emd == effectDefinition);
    }

    public void LearnEffectModifier(EffectDefinition effectDefinition)
    {
        _effectDefinitions.Add(effectDefinition);
    }
}
