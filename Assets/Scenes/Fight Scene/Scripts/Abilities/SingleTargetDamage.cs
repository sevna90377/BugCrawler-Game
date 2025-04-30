using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/SingleTargetDamage")]
public class SingleTargetDamage : Ability
{
    public int damageAmount;

    public override void Activate(Unit user, Unit target)
    {
        target.TakeDamage(damageAmount);
        Debug.Log($"{user.unitName} casts {abilityName} on {target.unitName}, dealing {damageAmount} damage!");
    }
}