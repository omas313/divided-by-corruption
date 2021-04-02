using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleAction
{
    public abstract ActionDefinition ActionDefinition { get; }
    public abstract bool IsValid { get; }
    
    public BattleActionType BattleActionType { get; set; }
    public BattleParticipant Performer { get; set; }
    public List<BattleParticipant> Targets { get; set; } = new List<BattleParticipant>();
    public bool HasTarget => Targets.Count > 0;

    ParticleSystem _energyParticles;

    public virtual IEnumerator PerformAction(List<PartyMember> party, List<Enemy> enemies)
    {
        yield return PreActionSetup();
        yield return Perform(party, enemies);
        yield return PostActionSetup();
    }

    protected abstract IEnumerator Perform(List<PartyMember> party, List<Enemy> enemies);

    protected ParticleSystem SpawnEnergyParticles()
    {
        var particles = ActionDefinition.SpawnEnergyParticles(Performer.CurrentPosition, Performer.transform);
        particles.Play();
        return particles;
    }

    protected virtual IEnumerator PreActionSetup()
    {
        if (ActionDefinition.HasEnergyParticles)
            _energyParticles = SpawnEnergyParticles();

        BattleEvents.InvokeBattleParticipantsTargetted(Targets);

        yield return new WaitForSeconds(ActionDefinition.DelayBeforeMotion);
        yield return ActionDefinition.AttackMotionType.PreActionMotion(Performer, Targets[0]); 
    }

    protected virtual IEnumerator PostActionSetup()
    {
        if (ActionDefinition.HasEnergyParticles)
        {
            _energyParticles.Stop();
            _energyParticles = null;
        }

        yield return ActionDefinition.AttackMotionType.PostActionMotion(Performer, Targets[0]);
    }

    bool AreTargetsEnemies() => Targets[0] is Enemy;
}
