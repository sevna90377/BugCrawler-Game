using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/BeeAbilities/HoneySpill")]
public class HoneySpill : Ability
{
    public override void Activate(Unit user, Unit target)
    {
        int healAmount = ApplyScaling(user, user.currentPower, powerScaling);
        target.Heal(healAmount);

        Debug.Log($"{user.unitName} casts {abilityName} on {target.unitName}");
    }
}
