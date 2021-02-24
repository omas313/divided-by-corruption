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
    
    
    public PartyMember CurrentPartyMember => _pendingPartyMembers[_currentPartyMemberIndex];

    List<PartyMember> _partyMembers;
    List<PartyMember> _pendingPartyMembers;
    int _currentPartyMemberIndex;

    List<Enemy> _enemies;

    List<BattleCommand> _allBattleCommands = new List<BattleCommand>();
    bool _playerConfirmed;

    public IEnumerator Init(List<PartyMember> partyMembers, List<Enemy> enemies)
    {
        _partyMembers = partyMembers;
        _pendingPartyMembers = new List<PartyMember>(_partyMembers);
        _enemies = enemies;
        _allBattleCommands.Clear();
        _playerConfirmed = false;

        Debug.Log("initialized command manager");

        yield return null;
    }

    public IEnumerator PlayerSetup()
    {
        _currentPartyMemberIndex = 0;
        
        while (true)
        {
            Debug.Log($"setting command for {CurrentPartyMember.Name}");

            
            if (Input.GetKeyDown(KeyCode.Tab) && _pendingPartyMembers.Count > 1) // change
            {
                _currentPartyMemberIndex = (_currentPartyMemberIndex + 1) % _pendingPartyMembers.Count;
                Debug.Log("changed");
                UpdateText();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                var enemy = GetRandomEnemy();
                _allBattleCommands.Add(new AttackCommand(CurrentPartyMember, CurrentPartyMember.RandomAttack, enemy));
                Debug.Log($"added command: {CurrentPartyMember.Name} to attack {enemy.Name}");

                _pendingPartyMembers.Remove(CurrentPartyMember);

                if (_pendingPartyMembers.Count != 0)
                    _currentPartyMemberIndex %= _pendingPartyMembers.Count;
                UpdateText();
            }

            if (Input.GetKeyDown(KeyCode.LeftControl) && _allBattleCommands.Count != 0)
                yield return RemoveLastCommand();

            if (Input.GetKeyDown(KeyCode.F) || _pendingPartyMembers.Count == 0) 
                yield return TryConfirm();

            if (_playerConfirmed)
                yield break;

            UpdateText();
            yield return null;
        }
    }

    IEnumerator TryConfirm()
    {
        yield return new WaitForSeconds(0.25f);

        while (true)
        {
            Debug.Log("confirm ??");


            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("confirmed");
                _playerConfirmed = true;
                break;
            }

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                Debug.Log("did not confirm");

                yield return RemoveLastCommand();
                break;
            }

            yield return null;
        }
    }

    IEnumerator RemoveLastCommand()
    {
        var lastCommand = _allBattleCommands[_allBattleCommands.Count - 1];
        _allBattleCommands.Remove(lastCommand);
        _pendingPartyMembers.Add(lastCommand.Actor as PartyMember);

        if (_pendingPartyMembers.Count == 1)
            _currentPartyMemberIndex = 0;
        else if (_pendingPartyMembers.Count != 0)
            _currentPartyMemberIndex %= _pendingPartyMembers.Count;

        UpdateText();
        yield return null;
    }

    IEnumerator ClearAllCommands()
    {
        _allBattleCommands.Clear();
        Debug.Log("clearing commands...");
        yield return null;
    }

    public IEnumerator EnemySetup()
    {
        foreach (var enemy in _enemies)
            _allBattleCommands.Add(new AttackCommand(enemy, enemy.RandomAttack, GetRandomPartyMember()));

        Debug.Log("setting up enemy commands...");
        yield return null;
    }

    public List<BattleCommand> GetOrderedCommands()
    {
        return _allBattleCommands
            .OrderByDescending(bc => bc.Actor.CharacterStats.CurrentSpeed)
            .ToList();
    }

    PartyMember GetRandomPartyMember() => _partyMembers[UnityEngine.Random.Range(0, _partyMembers.Count)];
    Enemy GetRandomEnemy() => _enemies[UnityEngine.Random.Range(0, _enemies.Count)];

    void Update()
    {
    }

    void UpdateText()
    {
        if (_pendingPartyMembers == null)
            return;

        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine($"pending:");
        foreach (var partyMember in _pendingPartyMembers)
        {
            var isCurrent = partyMember == CurrentPartyMember ? " [current]" : "";
            stringBuilder.AppendLine($"\t{partyMember.Name}{isCurrent}");
        }

        stringBuilder.AppendLine($"-------------");

        stringBuilder.AppendLine($"commands:");
        foreach (var command in _allBattleCommands)
            stringBuilder.AppendLine($"\t{command.Description}");

        _text.SetText(stringBuilder.ToString());
    }
}
