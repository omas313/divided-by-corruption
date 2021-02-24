using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleDataDefinition.asset", menuName = "Battle Data Definition")]
public class BattleDataDefinition : ScriptableObject
{
    public List<PartyMember> PlayerParty => _playerParty;
    public List<EnemyDefinition> Enemies => _enemiesDefinitions;
    

    [SerializeField] List<PartyMember> _playerParty;
    [SerializeField] List<EnemyDefinition> _enemiesDefinitions;
}
