using System.Collections.Generic;

public interface IEffectsManager
{
    List<Effect> Effects { get; }
    BattleParticipant Client { get; }
    void AddEffect(Effect effect);
    void ReduceEffectDurations();
}
