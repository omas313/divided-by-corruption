using System;
using UnityEngine;

[System.Serializable]
public class EnemyStats : CharacterStats
{
    public int BaseArmour => _baseArmour;
    public int CurrentArmour => _currentArmour;
    public float ArmourDefenseModifier => _armourDefenseModifier;

    [SerializeField] int _currentArmour = 10;
    [SerializeField] int _baseArmour = 10;
    [SerializeField] float _armourDefenseModifier = 0.5f;

    public void SetCurrentArmour(int amount) => _currentArmour = amount;
    public void IncreaseCurrentArmour(int amount) => _currentArmour = Math.Min(_baseArmour, CurrentArmour + amount);
    public void ReduceCurrentArmour(int amount) => _currentArmour = Math.Max(0, CurrentArmour - amount);
    public void RemoveArmourModifier() => ModifyDefenseModifier(-ArmourDefenseModifier);

    public void Init()
    {
        if (DefenseModifier != 0)
            ModifyDefenseModifier(ArmourDefenseModifier);
    }
}