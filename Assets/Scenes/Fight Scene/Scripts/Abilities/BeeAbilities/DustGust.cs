using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/BeeAbilities/DustGust")]
public class DustGust : Ability
{
    public override void Activate(Unit user, Unit target)
    {
        int healAmount = ApplyScaling(user, user.currentPower, powerScaling);
        target.Heal(healAmount);
        target.ApplyStatusEffect(new StatusEffect(StatusEffectType.Regeneration, effectDuration, user.currentPreservation, preservationScaling));

        Debug.Log($"{user.unitName} casts {abilityName} on {target.unitName}");
    }
}
