using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PikcupType", menuName = "Pikcup Type")]
public class PickupType : ScriptableObject
{
    public string Name => _name;
    public Sprite Sprite => _sprite;

    [SerializeField] string _name;

    [SerializeField] Sprite _sprite;    
}
