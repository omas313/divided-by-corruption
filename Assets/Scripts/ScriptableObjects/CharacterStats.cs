using System;
using UnityEngine;

[System.Serializable]
public class CharacterStats
{
    public int BaseHP => _baseHP;
    public int CurrentHP => _currentHP;

    public int BaseMP => _baseMP;
    public int CurrentMP => _currentMP;
    
    public int BaseSpeed => _baseSpeed;
    public int CurrentSpeed => _currentSpeed;

    public float MotionSpeed => _motionSpeed;


    [SerializeField] int _currentHP = 10;
    [SerializeField] int _baseHP = 10;

    [SerializeField] int _currentMP = 0;
    [SerializeField] int _baseMP = 10;
    
    [SerializeField] int _currentSpeed = 10;
    [SerializeField] int _baseSpeed = 10;

    [SerializeField] float _motionSpeed = 3f;


    public void SetCurrentHP(int amount) => _currentHP = amount;
    public void IncreaseCurrentHP(int amount) => _currentHP = Math.Min(_baseHP, CurrentHP + amount);
    public void ReduceCurrentHP(int amount) => _currentHP = Math.Max(0, CurrentHP - amount);

    public void SetCurrentMP(int amount) => _currentMP = amount;
    public void IncreaseCurrentMP(int amount) => _currentMP = Math.Min(_baseMP, CurrentMP + amount);
    public void ReduceCurrentMP(int amount) => _currentMP = Math.Max(0, CurrentMP - amount);
    public int TakeMP(int amount)
    {
        var amountTaken = CurrentMP - amount < 0 ? CurrentMP : amount;
        ReduceCurrentMP(amount);
        return amountTaken;
    }
    
    public bool HasEnoughMP(int amount) => _currentMP >= amount;

    public void SetCurrentSpeed(int amount) => _currentSpeed = amount;
    public void IncreaseCurrentSpeed(int amount) => _currentSpeed += amount;
    public void ReduceCurrentSpeed(int amount) => _currentSpeed += amount;
}
