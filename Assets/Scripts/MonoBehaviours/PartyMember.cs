using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyMember : BattleParticipant
{
    public override string Name => _name;
    public override CharacterStats CharacterStats => _stats;

    [SerializeField] string _name;
    [SerializeField] CharacterStats _stats;

    public override IEnumerator SetCommand(List<PartyMember> playerParty, List<Enemy> enemies)
    {
        Debug.Log($"{Name} SetCommand");
        yield return null;
    }

    public override IEnumerator Die()
    {
        Debug.Log($"{Name} Die");
        yield return null;
    }

    public override IEnumerator ReceiveAttack(BattleAttack attack)
    {
        Debug.Log($"{Name} ReceiveAttack");
        yield return null;
    }
}
