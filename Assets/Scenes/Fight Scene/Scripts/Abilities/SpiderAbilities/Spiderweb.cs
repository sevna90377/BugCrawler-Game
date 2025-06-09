using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/SpiderAbilities/Spiderweb")]
public class Spiderweb : Ability
{
    public override void Activate(Unit caster, Unit target)
    {
        List<Tuple<StatType, int, float>> debuff = new()
        {
            new Tuple<StatType, int, float>(StatType.Speed, caster.currentAggression, aggressionScaling)
        };

        target.ApplyStatusEffect(new StatusEffect(StatusEffectType.Debuff, effectDuration, debuff, false));

        Debug.Log($"{caster.unitName} casts {abilityName} on {target.unitName}");
    }
}
