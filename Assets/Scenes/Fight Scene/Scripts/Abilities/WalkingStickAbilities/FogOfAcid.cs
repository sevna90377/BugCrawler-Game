using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/WalkingStickAbilities/FogOfAcid")]
public class FogofAcid : Ability
{
    public override void Activate(Unit caster, Unit target)
    {

        target.ApplyStatusEffect(new StatusEffect(StatusEffectType.Poison, effectDuration, caster.currentAggression, aggressionScaling));

        Debug.Log($"{caster.unitName} casts {abilityName} on {target.unitName}");
    }
}
