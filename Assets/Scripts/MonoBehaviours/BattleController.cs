using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using System.Linq;
using System.Collections;

public class BattleController : MonoBehaviour
{
    // get from somewhere later
    [SerializeField] List<Enemy> _enemies;
    [SerializeField] List<PartyMember> _partyMembers;

    List<BattleParticipant> _battleParticipants;
    List<PartyMember> _activePlayerParty;
    List<Enemy> _activeEnemies;
    CommandManager _commandManager;
    CommandPlayer _commandPlayer;
    bool _hasBattleStarted;


    void StartBattle() => StartCoroutine(TurnBasedBattle());

    IEnumerator TurnBasedBattle()
    {
        _hasBattleStarted = true;

        yield return new WaitForSeconds(0.25f); // for UI to sub to events

        _activePlayerParty = new List<PartyMember>(_partyMembers);
        _activeEnemies = new List<Enemy>(_enemies);

        InitBattleParticipants(_partyMembers, _enemies);
        BattleEvents.InvokeBattleStarted(_partyMembers, _enemies);

        int loopNumber = 0;
        while (true)
        {
            loopNumber++;
            // Debug.Log($"battle loop: {loopNumber}");

            yield return _commandManager.Init(_activePlayerParty, _activeEnemies);
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

        Debug.Log("battle ended");
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
            if (command.Actor.IsDead || command.Target.IsDead)
                continue;

            yield return command.Execute();
            yield return new WaitForSeconds(1f);

            yield return CheckDeadParticipants();
        }
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
                _activePlayerParty.Remove(participant as PartyMember);
        }
    }

    IEnumerator BattleVictory()
    {
        Debug.Log("Battle ended in victory");

        yield return null;
    }

    IEnumerator BattleLoss()
    {
        Debug.Log("Battle ended in loss");

        yield return null;
    }

    bool AllEnemiesAreDead() => _activeEnemies.Count == 0;

    bool AllPartyMembersAreDead() => _activePlayerParty.Count == 0;

    private void Awake()
    {
        StartBattle();
        _commandManager = GetComponent<CommandManager>();
        _commandPlayer = GetComponent<CommandPlayer>();
    }
}
