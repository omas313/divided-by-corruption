using System.Collections.Generic;

public class EffectsManager
{
    public List<EffectDefinition> Effects { get; private set; } = new List<EffectDefinition>();
    public BattleParticipant Client { get; private set; }

    public EffectsManager(BattleParticipant client)
    {
        Client = client;
    }

    // todo: to be called by ComboManager or TurnManager or BattleParticipant ?
    public void AddEffect(EffectDefinition effectDefinition)
    {
        Effects.Add(effectDefinition);
        effectDefinition.ApplyEffect(Client);
    }

    // todo: To be called after action is performed i.e. before turn ends
    public void ReduceEffectDurations()
    {
        foreach (var effect in Effects)
            effect.ReduceDuration();

        Effects.RemoveAll(e => e.HasFinished);
    }
}
