using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using System.Linq;
using System.Collections;

public class BattleController : MonoBehaviour
{
    [SerializeField] Transform _battleParticipantsParent;

    List<Enemy> _enemies = new List<Enemy>();
    List<PartyMember> _partyMembers = new List<PartyMember>();
    List<BattleParticipant> _battleParticipants;
    List<PartyMember> _activePartyMembers;
    List<Enemy> _activeEnemies;
    CommandManager _commandManager;

    public void InitBattleAndStart(List<PartyMember> partyMembersPrefabs, List<Enemy> enemiesPrefabs)
    {
        foreach (var partyMemberPrefab in partyMembersPrefabs)
            _partyMembers.Add(Instantiate(partyMemberPrefab, transform.position, Quaternion.identity, _battleParticipantsParent));
            
        foreach (var enemyPrefab in enemiesPrefabs)
            _enemies.Add(Instantiate(enemyPrefab, transform.position, Quaternion.identity, _battleParticipantsParent));

        StartBattle();
    }

    void StartBattle() => StartCoroutine(TurnBasedBattle());

    IEnumerator TurnBasedBattle()
    {
        yield return new WaitForSeconds(0.25f); // for UI to sub to events

        _activePartyMembers = new List<PartyMember>(_partyMembers);
        _activeEnemies = new List<Enemy>(_enemies);

        InitBattleParticipants(_partyMembers, _enemies);
        BattleEvents.InvokeBattleStarted(_partyMembers, _enemies);

        int loopNumber = 0;
        while (true)
        {
            loopNumber++;
            // Debug.Log($"battle loop: {loopNumber}");

            yield return _commandManager.Init(_activePartyMembers, _activeEnemies);
            yield return _commandManager.PlayerSetup();     
            yield return _commandManager.EnemySetup();

            yield return PlayCommands(_commandManager.GetOrderedCommands());

            if (AllEnemiesAreDead())
            {
                yield return BattleVictory();
                break;
            }

            if (AllPartyMembersAreDead())
            {
                yield return BattleLoss();
                break;
            }
        }

        // Debug.Log("battle ended");
    }

    void InitBattleParticipants(List<PartyMember> partyMembers, List<Enemy> enemies)
    {
        _battleParticipants = new List<BattleParticipant>();
        
        foreach (var partyMember in _partyMembers)
            _battleParticipants.Add(partyMember); 
        
        foreach (var enemy in _enemies)
            _battleParticipants.Add(enemy); 

        _battleParticipants = _battleParticipants.OrderByDescending(bp => bp.CharacterStats.CurrentSpeed).ToList();
        
        BattleEvents.InvokePartyUpdated(_partyMembers, null);        
    }

    IEnumerator PlayCommands(List<BattleCommand> battleCommands)
    {
        foreach (var command in battleCommands)
        {
            if (command.Actor.IsDead)
                continue;

            if (command.Target.IsDead)
                command.Target = GetAnother(command.Target);

            if (command.Target == null)
                continue;

            yield return command.Execute();
            yield return CheckDeadParticipants();
        }
    }

    BattleParticipant GetAnother(BattleParticipant target)
    {
        if (target is Enemy && _activeEnemies.Count == 0)
            return null;
        
        if (target is Enemy)
            return _activeEnemies.FirstOrDefault(e => e != target);

        if (target is PartyMember && _activePartyMembers.Count == 0)
            return null;
        
        if (target is PartyMember)
            return _activeEnemies.FirstOrDefault(pm => pm != target);

        return null;
    }

    IEnumerator CheckDeadParticipants()
    {
        var deadParticipants = new List<BattleParticipant>();
        foreach (var participant in _battleParticipants)
            if (participant.IsDead)
                deadParticipants.Add(participant);

        if (deadParticipants.Count == 0)
            yield break;

        foreach (var participant in deadParticipants)
        {
            yield return participant.Die();
            
            _battleParticipants.Remove(participant);

            if (participant is Enemy)
                _activeEnemies.Remove(participant as Enemy);
            else
                _activePartyMembers.Remove(participant as PartyMember);
        }
    }

    IEnumerator BattleVictory()
    {
        yield return new WaitForSeconds(2f);
        BattleEvents.InvokeBattleEnded(hasWon: true);
        // Debug.Log("Battle ended in victory");

    }

    IEnumerator BattleLoss()
    {
        yield return new WaitForSeconds(2f);
        BattleEvents.InvokeBattleEnded(hasWon: false);

    }

    bool AllEnemiesAreDead() => _activeEnemies.Count == 0;

    bool AllPartyMembersAreDead() => _activePartyMembers.Count == 0;

    private void Awake()
    {
        // StartBattle();
        _commandManager = GetComponent<CommandManager>();
    }
}
