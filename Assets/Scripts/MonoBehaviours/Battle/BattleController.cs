using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using System.Linq;
using System.Collections;

public class BattleController : MonoBehaviour
{
    [Header("Manual Setup")] 
    [SerializeField] bool _manualSetup;
    [SerializeField] List<PartyMember> _usedParty;
    [SerializeField] List<Enemy> _usedEnemies;

    [SerializeField] Transform _battleParticipantsParent;

    BattleParticipant CurrentBattleParticipant => _battleParticipants[_currentIndex];
    int _currentIndex;

    List<Enemy> _enemies = new List<Enemy>();
    List<PartyMember> _party = new List<PartyMember>();
    List<BattleParticipant> _battleParticipants;
    List<PartyMember> _activeParty;
    List<Enemy> _activeEnemies;
    TurnManager _turnManager;

    public void InitBattleAndStart(List<PartyMember> partyMembersPrefabs, List<Enemy> enemiesPrefabs)
    {
        foreach (var partyMemberPrefab in partyMembersPrefabs)
            _party.Add(Instantiate(partyMemberPrefab, transform.position, Quaternion.identity, _battleParticipantsParent));
            
        foreach (var enemyPrefab in enemiesPrefabs)
            _enemies.Add(Instantiate(enemyPrefab, transform.position, Quaternion.identity, _battleParticipantsParent));

        StartCoroutine(StartBattle(_party, _enemies));
    }

    IEnumerator StartBattle(List<PartyMember> partyMembers, List<Enemy> enemies)
    {
        InitBattleParticipants(partyMembers, enemies);
        yield return new WaitForSeconds(0.15f); // for UI to sub to events
        BattleEvents.InvokeBattleStarted(_party, _enemies);
        BattleEvents.InvokePartyUpdated(_party, null);
        StartCoroutine(TurnBasedBattle());
    }

    void InitBattleParticipants(List<PartyMember> party, List<Enemy> enemies)
    {
        _battleParticipants = new List<BattleParticipant>();
        _party = new List<PartyMember>(party);
        _enemies = new List<Enemy>(enemies);
        
        foreach (var partyMember in _party)
            _battleParticipants.Add(partyMember); 
        
        foreach (var enemy in _enemies)
            _battleParticipants.Add(enemy); 

        _activeParty = new List<PartyMember>(_party);
        _activeEnemies = new List<Enemy>(_enemies);

        _battleParticipants = _battleParticipants.OrderByDescending(bp => bp.CharacterStats.CurrentSpeed).ToList();
    }

    IEnumerator TurnBasedBattle()
    {
        int loopNumber = 0;
        while (true)
        {
            loopNumber++;
            // Debug.Log($"battle loop: {loopNumber}");

            BattleEvents.InvokeCurrentPartyMemberChanged(CurrentBattleParticipant is PartyMember ? CurrentBattleParticipant as PartyMember : null);

            yield return _turnManager.Manage(CurrentBattleParticipant);     

            _currentIndex = (_currentIndex + 1) % _battleParticipants.Count;
            yield return CheckDeadParticipants();

            if (AllEnemiesAreDead())
            {
                StartCoroutine(BattleVictory());
                break;
            }

            if (AllPartyMembersAreDead())
            {
                StartCoroutine(BattleLoss());
                break;
            }
        }

        Debug.Log("battle ended");
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

        yield return KillAndRemoveDeadParticipants(deadParticipants);

        _currentIndex = _battleParticipants.IndexOf(nextParticipant);
    }

    IEnumerator KillAndRemoveDeadParticipants(List<BattleParticipant> deadParticipants)
    {
        foreach (var deadParticipant in deadParticipants)
        {
            yield return deadParticipant.Die();
            _battleParticipants.Remove(deadParticipant);

            if (deadParticipant is Enemy)
                _activeEnemies.Remove(deadParticipant as Enemy);
            else
                _activeParty.Remove(deadParticipant as PartyMember);
        }
    }
    IEnumerator BattleVictory()
    {
        yield return new WaitForSeconds(2f);
        // BattleEvents.InvokeBattleEnded(hasWon: true);
        Debug.Log("Battle ended in victory");

    }

    IEnumerator BattleLoss()
    {
        yield return new WaitForSeconds(2f);
        // BattleEvents.InvokeBattleEnded(hasWon: false);
        Debug.Log("Battle ended in loss");
    }

    bool AllEnemiesAreDead() => _activeEnemies.Count == 0;

    bool AllPartyMembersAreDead() => _activeParty.Count == 0;

    void Awake()
    {
        _turnManager = GetComponent<TurnManager>();

        if (_manualSetup)
            StartCoroutine(StartBattle(_usedParty, _usedEnemies));
    }
}
