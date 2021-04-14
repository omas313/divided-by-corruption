using System;
using System.Collections.Generic;
using System.Linq;

public class Combo
{
    public bool HasStarted { get; private set; }
    public bool IsBroken { get; private set; }
    public bool HasFinished => !HasParticipants;
    public bool HasParticipants => _participants.Count > 0;
    public int TotalDamage { get; private set; }
    public PartyMember NextParticipant => _participants.First.Value;
    public int ParticipantsCount => _participants.Count;
    public int EffectsCount => _effects.Count;
    public int TargetsCount => _targets.Count;
    public List<PartyMember> Participants => _participants.ToList();
    public List<Enemy> Targets => _targets;
    public List<Effect> Effects => _effects;

    LinkedList<PartyMember> _participants = new LinkedList<PartyMember>();
    List<Effect> _effects = new List<Effect>();
    List<Enemy> _targets = new List<Enemy>();

    readonly BattleActionType[] _comboTrialBattleActions = 
    {
        BattleActionType.Attack,
        BattleActionType.Special
    };

    readonly BattleActionType[] _comboBreakingBattleActions = 
    {
        BattleActionType.Absorb,
        BattleActionType.Defend,
        BattleActionType.None,
    };

    public Combo(PartyMember comboStarterPartyMember)
    {
        _participants.AddFirst(comboStarterPartyMember);
    }

    public void Start() => HasStarted = true;

    public void Break()
    {
        IsBroken = true;
        Clear();
    }

    public bool ShouldPerformComboTrial(PartyMember currentPartyMember, BattleAction battleAction) =>
        HasParticipants
        && _participants.Count > 1
        && !battleAction.HasFailed
        && _comboTrialBattleActions.Contains(battleAction.BattleActionType);

    public bool ShouldBreakCombo(PartyMember currentPartyMember, BattleAction battleAction) =>
        IsFirstParticipant(currentPartyMember)
        && (battleAction.HasFailed
        || _comboBreakingBattleActions.Contains(battleAction.BattleActionType));
        
    public void AddFirstParticipant(PartyMember participant) => _participants.AddFirst(participant);
    public void AddParticipantsInOrder(PartyMember firstParticipant, PartyMember secondParticipant)
    {
        _participants.AddFirst(firstParticipant);
        _participants.AddLast(secondParticipant);
    }
    public void RemoveParticipant(PartyMember participant) => _participants.Remove(participant);
    public PartyMember GetParticipantAfter(PartyMember participant) => _participants.Find(participant).Next.Value;
    public bool IsParticipant(PartyMember partyMember) => _participants.Find(partyMember) != null;
    public bool IsFirstParticipant(PartyMember partyMember) => _participants.First.Value == partyMember;

    public void AddEffect(Effect effect) => _effects.Add(effect);
    public void RemoveEffect(Effect effect) => _effects.Remove(effect);

    public void AddTarget(Enemy enemy) => _targets.Add(enemy);
    public void AddTargets(List<Enemy> enemies)
    {
        foreach(var enemy in enemies)
            if (!_targets.Contains(enemy))
                _targets.Add(enemy);
    }
    public void RemoveTarget(Enemy enemy) => _targets.Remove(enemy);

    public void AddDamage(int damage) => TotalDamage += damage;

    public void Clear()
    {
        _targets?.Clear();
        _targets = null;
        _participants?.Clear();
        _participants = null;
        _effects?.Clear();
        _effects = null;
    }
}
