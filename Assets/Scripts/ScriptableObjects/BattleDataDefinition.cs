using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleDataDefinition.asset", menuName = "Battle Data Definition")]
public class BattleDataDefinition : ScriptableObject
{
    public List<PartyMember> PlayerParty => _playerParty;
    public List<Enemy> Enemies => _enemies;
    

    [SerializeField] List<PartyMember> _playerParty;
    [SerializeField] List<Enemy> _enemies;
}
