using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/AntWarriorAbilities/PheromoneSignaling")]

public class PheromoneSignaling : Ability
{
    public override void Activate(Unit caster, Unit target)
    {

        List<Tuple<StatType, int, float>> debuffSelf = new()
        {
            new Tuple<StatType, int, float>(StatType.Speed, caster.currentAggression, aggressionScaling)
        };

        caster.ApplyStatusEffect(new StatusEffect(StatusEffectType.Debuff, effectDuration, debuffSelf, false));

        List<Tuple<StatType, int, float>> buffTarget = new()
        {
            new Tuple<StatType, int, float>(StatType.Speed, caster.currentPreservation, preservationScaling)
        };

        target.ApplyStatusEffect(new StatusEffect(StatusEffectType.Buff, effectDuration, buffTarget, false));


        Debug.Log($"{caster.unitName} casts {abilityName} on {target.unitName}");
    }
}
