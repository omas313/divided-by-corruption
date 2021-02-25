using UnityEngine;

[CreateAssetMenu(fileName = "DamageType.asset", menuName = "Damage Type")]
public class DamageType : ScriptableObject
{
    public string Name => _name;
    public Color Color => _color;
    
    [SerializeField] string _name;
    [SerializeField] Color _color;
}
