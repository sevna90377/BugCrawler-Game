using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


[CreateAssetMenu(menuName = "Abilities/SpiderAbilities/PoisonSpit")]
public class PoisonSpit : Ability
{
    public override void Activate(Unit caster, Unit target)
    {
        int dmg = ApplyScaling(caster, caster.currentStrength, strengthScaling);
        target.TakeDamage(dmg);

        target.ApplyStatusEffect(new StatusEffect(StatusEffectType.Poison, effectDuration, caster.currentAggression, aggressionScaling));

        Debug.Log($"{caster.unitName} casts {abilityName} on {target.unitName}");
    }
}
