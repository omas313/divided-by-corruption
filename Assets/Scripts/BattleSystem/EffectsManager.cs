using System.Collections.Generic;

public class EffectsManager
{
    public List<Effect> Effects { get; private set; } = new List<Effect>();
    public BattleParticipant Client { get; private set; }

    public EffectsManager(BattleParticipant client)
    {
        Client = client;
    }

    public void AddEffect(Effect effect)
    {
        Effects.Add(effect);
        effect.ApplyEffect();
    }

    public void ReduceEffectDurations()
    {
        if (Effects.Count == 0)
            return;

        foreach (var effect in Effects)
            effect.ReduceDuration();

        Effects.RemoveAll(e => e.HasFinished);
    }
}
