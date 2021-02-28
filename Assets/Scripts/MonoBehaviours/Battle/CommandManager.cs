using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using System.Text;

public class CommandManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _text;
    
    [SerializeField] BattleParticipantMarker _currentActorMarker;
    
    public PartyMember CurrentPartyMember => _pendingPartyMembers[_currentPartyMemberIndex];

    List<BattleCommand> _allBattleCommands = new List<BattleCommand>();
    List<Enemy> _enemies;
    List<PartyMember> _partyMembers;
    List<PartyMember> _pendingPartyMembers;
    int _currentPartyMemberIndex;

    bool _playerConfirmedCommands;
    bool _attackSelected;

    public IEnumerator Init(List<PartyMember> partyMembers, List<Enemy> enemies)
    {
        _partyMembers = partyMembers;
        _pendingPartyMembers = new List<PartyMember>(_partyMembers);
        _enemies = enemies;
        _allBattleCommands.Clear();
        _playerConfirmedCommands = false;
        _attackSelected = false;

        // Debug.Log("initialized command manager");

        yield return null;
    }

    public IEnumerator EnemySetup()
    {
        foreach (var enemy in _enemies)
            _allBattleCommands.Add(new AttackCommand(enemy, enemy.RandomAttack, GetRandomPartyMember()));

        yield return null;
    }

    public List<BattleCommand> GetOrderedCommands()
    {
        return _allBattleCommands
            .OrderByDescending(bc => bc.Actor.CharacterStats.CurrentSpeed)
            .ToList();
    }

    public IEnumerator PlayerSetup()
    {
        _currentPartyMemberIndex = 0;
        BattleEvents.InvokePartyUpdated(_partyMembers, CurrentPartyMember);        
        _currentActorMarker.PlaceAt(CurrentPartyMember.transform.position);
        _partyMembers.ForEach(pm => BattleEvents.InvokePartyMemberCommandUnset(pm));

        while (true)
        {
            // Debug.Log($"setting command for {CurrentPartyMember.Name}");

            if (Input.GetButtonDown("Change") && _pendingPartyMembers.Count > 1) 
                yield return ChangeActivePartyMember();
            
            if (Input.GetButtonDown("Back") && _allBattleCommands.Count != 0)
                yield return RemoveLastCommand();

            if (_pendingPartyMembers.Count == 0) 
                yield return TryConfirm();

            if (_playerConfirmedCommands)
                yield break;

            yield return null;
        }
    }

    void AddAttackCommand(BattleParticipant target)
    {
        if (!_attackSelected || target == null)
        {
            Debug.Log($"Trying to AddAttackCommand with _attackSelected:{_attackSelected}, target:{target}");
            return;
        }

        var command = new AttackCommand(CurrentPartyMember, CurrentPartyMember.RandomAttack, target);
        _allBattleCommands.Add(command);
        BattleEvents.InvokePartyMemberCommandSet(CurrentPartyMember);

        if (IsThisLastPendingPartyMember())
        {
            _pendingPartyMembers.Remove(CurrentPartyMember);
            BattleEvents.InvokePartyUpdated(_partyMembers, null);    
            _currentActorMarker.Hide();
            return;
        }
        
        SetNextActivePartymember();
    }

    bool IsThisLastPendingPartyMember() => _pendingPartyMembers.Count == 1;

    void SetNextActivePartymember()
    {
        var nextIndex = (_currentPartyMemberIndex + 1) % _pendingPartyMembers.Count;
        var nextPartyMember = _pendingPartyMembers[nextIndex];
        
        _pendingPartyMembers.Remove(CurrentPartyMember);

        _currentPartyMemberIndex = _pendingPartyMembers.IndexOf(nextPartyMember);
        if (_pendingPartyMembers.Count != 0)
            _currentPartyMemberIndex %= _pendingPartyMembers.Count;

        BattleEvents.InvokePartyUpdated(_partyMembers, CurrentPartyMember);     
        _currentActorMarker.PlaceAt(CurrentPartyMember.transform.position);
    }


    IEnumerator ChangeActivePartyMember()
    {
        _currentPartyMemberIndex = (_currentPartyMemberIndex + 1) % _pendingPartyMembers.Count;
        BattleEvents.InvokePartyUpdated(_partyMembers, CurrentPartyMember);       

        _currentActorMarker.PlaceAt(CurrentPartyMember.transform.position);

        yield return null;
    }

    IEnumerator TryConfirm()
    {
        BattleEvents.InvokePartyUpdated(_partyMembers, null);
        BattleEvents.InvokeRequestedCommandsConfirmation();

        yield return new WaitUntil(() => _playerConfirmedCommands || _pendingPartyMembers.Count != 0);
    }

    IEnumerator RemoveLastCommand()
    {
        var lastCommand = _allBattleCommands[_allBattleCommands.Count - 1];
        var partyMember = lastCommand.Actor as PartyMember;
        _allBattleCommands.Remove(lastCommand);
        _pendingPartyMembers.Add(partyMember);

        BattleEvents.InvokePartyMemberCommandUnset(partyMember);

        if (_pendingPartyMembers.Count == 1)
            _currentPartyMemberIndex = 0;
        else if (_pendingPartyMembers.Count != 0)
            _currentPartyMemberIndex %= _pendingPartyMembers.Count;

        BattleEvents.InvokePartyUpdated(_partyMembers, CurrentPartyMember);  

        _currentActorMarker.PlaceAt(CurrentPartyMember.transform.position);

        yield return null;
    }

    PartyMember GetRandomPartyMember() => _partyMembers[UnityEngine.Random.Range(0, _partyMembers.Count)];

    Enemy GetRandomEnemy() => _enemies[UnityEngine.Random.Range(0, _enemies.Count)];

    // void OnSkillSelected(SkillDefinition skillDefinition) => _skillSelected = skillDefinition;

    void OnCommandsConfirmed() => _playerConfirmedCommands = true;
    void OnCommandsNotConfirmed() => StartCoroutine(RemoveLastCommand());

    void OnAttackSelected() => _attackSelected = true;

    void OnTargetSelected(BattleParticipant target)
    {
        if (_attackSelected)
            AddAttackCommand(target);
        // else if (_skillSelected != null)
        //     AddSkillCommand(target);

        _attackSelected = false;
        // _skillSelected = null;
    } 

    void Awake()
    {
        BattleEvents.TargetSelected += OnTargetSelected;    
        BattleEvents.AttackSelected += OnAttackSelected;    
        // BattleEvents.SkillSelected += OnSkillSelected;    
        BattleEvents.CommandsConfirmed += OnCommandsConfirmed;
        BattleEvents.CommandsNotConfirmed += OnCommandsNotConfirmed;
    }


    void OnDestroy()
    {
        BattleEvents.TargetSelected -= OnTargetSelected;    
        BattleEvents.AttackSelected -= OnAttackSelected;    
        // BattleEvents.SkillSelected -= OnSkillSelected;    
        BattleEvents.CommandsConfirmed -= OnCommandsConfirmed;
        BattleEvents.CommandsNotConfirmed -= OnCommandsNotConfirmed;
    }
}
