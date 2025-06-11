using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/WalkingStickAbilities/Camuflage")]
public class Camuflage : Ability
{
    public override void Activate(Unit caster, Unit target)
    {

        target.ApplyStatusEffect(new StatusEffect(StatusEffectType.Regeneration, effectDuration, caster.currentPreservation, preservationScaling));

        Debug.Log($"{caster.unitName} casts {abilityName} on {target.unitName}");
    }
}
