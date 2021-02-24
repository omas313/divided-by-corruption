using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackType.asset", menuName = "Attack Type")]
public class AttackType : ScriptableObject
{
    public string Name => _name;
    public Color Color => _color;
    
    [SerializeField] string _name;
    [SerializeField] Color _color;
}