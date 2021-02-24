using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using System.Linq;
using System.Collections;

public class BattleController : MonoBehaviour
{
    public BattleParticipant CurrentParticipant => _battleParticipants[_currentIndex];

    // get from somewhere later
    [SerializeField] List<Enemy> _enemies;
    [SerializeField] List<PartyMember> _partyMembers;

    List<BattleParticipant> _battleParticipants;
    List<PartyMember> _activePlayerParty;
    List<Enemy> _activeEnemies;
    CommandManager _commandManager;
    CommandPlayer _commandPlayer;
    int _currentIndex;
    bool _hasBattleStarted;


    void StartBattle() => StartCoroutine(TurnBasedBattle());

    IEnumerator TurnBasedBattle()
    {
        _hasBattleStarted = true;

        yield return new WaitForSeconds(0.25f); // for UI to sub to events

        _activePlayerParty = new List<PartyMember>(_partyMembers);
        _activeEnemies = new List<Enemy>(_enemies);

        InitBattleParticipants(_partyMembers, _enemies);

        _currentIndex = 0;

        int loopNumber = 0;
        while (true)
        {
            loopNumber++;
            Debug.Log($"battle loop: {loopNumber}");

            yield return _commandManager.Init(_partyMembers, _enemies);
            yield return _commandManager.PlayerSetup();     
            yield return _commandManager.EnemySetup();

            // play commands here one by one so we can check for deaths
            yield return _commandPlayer.Play(_commandManager.GetOrderedCommands());

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

    IEnumerator CheckDeadParticipants()
    {
        var deadParticipants = new List<BattleParticipant>();
        foreach (var participant in _battleParticipants)
            if (participant.IsDead)
                deadParticipants.Add(participant);

        if (deadParticipants.Count == 0)
            yield break;

        var nextParticipant = _battleParticipants[_currentIndex];
        while (nextParticipant.IsDead)
        {
            _currentIndex = (_currentIndex + 1) % _battleParticipants.Count;
            nextParticipant = _battleParticipants[_currentIndex];
        }

        yield return KillAndRemoveParticipants(deadParticipants);

        _currentIndex = _battleParticipants.IndexOf(nextParticipant);
    }

    IEnumerator KillAndRemoveParticipants(List<BattleParticipant> deadParticipants)
    {
        foreach (var deadParticipant in deadParticipants)
        {
            yield return deadParticipant.Die();
            
            _battleParticipants.Remove(deadParticipant);

            if (deadParticipant is Enemy)
                _activeEnemies.Remove(deadParticipant as Enemy);
            else
                _activePlayerParty.Remove(deadParticipant as PartyMember);
        }
    }

    bool AllEnemiesAreDead() => _activeEnemies.Count == 0;

    bool AllPartyMembersAreDead() => _activePlayerParty.Count == 0;

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

    private void Awake()
    {
        StartBattle();
        _commandManager = GetComponent<CommandManager>();
        _commandPlayer = GetComponent<CommandPlayer>();
    }
}
