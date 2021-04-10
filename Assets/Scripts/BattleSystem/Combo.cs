using System;
using System.Collections.Generic;
using System.Linq;

public class Combo
{
    public bool HasStarted { get; private set; }
    public bool HasFinished => !HasParticipants;
    public bool HasParticipants => _participants.Count > 0;
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

    public void StartCombo() => HasStarted = true;

    public bool ShouldPerformComboTrial(PartyMember currentPartyMember, BattleActionType battleActionType) =>
        HasParticipants
        && _participants.Count > 1
        && _comboTrialBattleActions.Contains(battleActionType);

    public bool ShouldBreakCombo(PartyMember currentPartyMember, BattleActionType battleActionType) =>
        HasStarted 
        && IsParticipant(currentPartyMember)
        && _comboBreakingBattleActions.Contains(battleActionType);
        
    public void AddFirstParticipant(PartyMember participant) => _participants.AddFirst(participant);
    public void AddParticipantsInOrder(PartyMember firstParticipant, PartyMember secondParticipant)
    {
        _participants.AddFirst(firstParticipant);
        _participants.AddLast(secondParticipant);
    }
    public void RemoveParticipant(PartyMember participant) => _participants.Remove(participant);
    public PartyMember GetParticipantAfter(PartyMember participant) => _participants.Find(participant).Next.Value;
    public bool IsParticipant(PartyMember partyMember) => _participants.Find(partyMember) != null;

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


    public void Clear()
    {
        _targets.Clear();
        _participants.Clear();
        _effects.Clear();
    }
}
