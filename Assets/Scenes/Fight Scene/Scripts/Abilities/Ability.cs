using UnityEngine;
public abstract class Ability : ScriptableObject
{
    public string abilityName;
    public Sprite icon;
    public bool canTargetFriendly;

    public abstract void Activate(Unit caster, Unit target);
}