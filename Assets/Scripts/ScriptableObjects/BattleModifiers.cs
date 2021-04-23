using System;

[System.Serializable]
public class BattleModifiers
{
    public float OverallDamageModifier { get; private set; } = 1f;
    public float HealthDamageModifier { get; private set; } = 1f;
    public float ArmourDamageModifier { get; private set; } = 1f;
    public float DefenseModifier { get; private set; } = 1f;
    public SegmentModifier NormalSegmentModifier { get; private set; } = new SegmentModifier();
    public SegmentModifier CriticalSegmentModifier { get; private set; } = new SegmentModifier();
    public bool HasSplashDamage { get; private set; } = false;

    public void ModifyOverallDamageModifier(float percentage) => OverallDamageModifier += percentage;
    public int ApplyOverallDamageModifier(int damage) => (int)Math.Ceiling(damage * OverallDamageModifier);

    public void ModifyHealthDamageModifier(float percentage) => HealthDamageModifier += percentage;
    public int ApplyHealthDamageModifier(int damage) => (int)Math.Ceiling(damage * HealthDamageModifier);

    public void ModifyArmourDamageModifier(float percentage) => ArmourDamageModifier += percentage;
    public int ApplyArmourDamageModifier(int damage) => (int)Math.Ceiling(damage * ArmourDamageModifier);
    
    public void ModifyDefenseModifier(float percentage) => DefenseModifier += percentage;
    public int ApplyDefenseModifier(int damage)
    {
        // Modifier > 1 ==> reduce damage received
        // Modifier < 1 ==> increase damage received
        var modification = 0f;

        if (DefenseModifier > 1f)
            modification = (1f - DefenseModifier) * damage;
        else if (DefenseModifier < 1f)
            modification = -(DefenseModifier - 1f) * damage;
        else
            modification = 0f;

        return (int)Math.Ceiling(damage + modification);
    }
    
    public void ModifyNormalSegmentValueModifier(float percentage) => NormalSegmentModifier.Value += percentage;
    public void ModifyNormalSegmentWidthModifier(float percentage) => NormalSegmentModifier.Width += percentage;

    public void ModifyCriticalSegmentValueModifier(float percentage) => CriticalSegmentModifier.Value += percentage;
    public void ModifyCriticalSegmentWidthModifier(float percentage) => CriticalSegmentModifier.Width += percentage;

    public void ActivateSplashDamage() => HasSplashDamage = true;
    public void DeactivateSplashDamage() => HasSplashDamage = false;

}