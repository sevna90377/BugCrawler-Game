using UnityEngine;
public abstract class Ability : ScriptableObject
{
    public string abilityName;
    public Sprite icon;

    public CanTarget canTarget;

    public float strengthScaling;
    public float powerScaling;
    public float aggressionScaling;
    public float preservationScaling;

    public bool canTargetFriendly;
    public int effectDuration;

    public abstract void Activate(Unit caster, Unit target);

    public enum CanTarget
    {
        Self,
        AnySingle,
        FrontlaneSingle,
        FrontlaneBoth,
        BacklaneSingle,
        BacklaneBoth
    }

    protected int ApplyScaling(Unit caster, float amount, float scaling)
    {
        return Mathf.RoundToInt(amount * scaling);
    }
}