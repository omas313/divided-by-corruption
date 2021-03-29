using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbsorbDefinition.asset", menuName = "Battle/Action Definition/Absorb Definition")]
public class AbsorbDefinition : ActionDefinition
{
    public int Percentage => _percentage;

    [SerializeField] int _percentage;
}
