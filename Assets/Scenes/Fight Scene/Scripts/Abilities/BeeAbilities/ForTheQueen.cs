using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/BeeAbilities/ForTheQueen")]
public class ForTheQueen : Ability
{
    public override void Activate(Unit user, Unit target)
    {
        int damage = ApplyScaling(user, user.currentStrength, strengthScaling);
        target.TakeDamage(damage);
        user.TakeDamage(999);

        Debug.Log($"{user.unitName} casts {abilityName} on {target.unitName}");
    }
}
