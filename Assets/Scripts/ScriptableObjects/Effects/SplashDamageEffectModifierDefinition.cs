using UnityEngine;

[CreateAssetMenu(fileName = "SplashDamageEffectModifierDefinition.asset", menuName = "Battle/Effects/Splash Damage Effect Modifier Definition")]
public class SplashDamageEffectModifierDefinition : EffectModifierDefinition
{    
    public override void Apply(BattleParticipant target, float value)
    {
        target.CharacterStats.ActivateSplashDamage();
    }

    public override void Undo(BattleParticipant target, float value)
    {
        target.CharacterStats.DeactivateSplashDamage();
    }

    public override string Stringify(float value) => "";
}
