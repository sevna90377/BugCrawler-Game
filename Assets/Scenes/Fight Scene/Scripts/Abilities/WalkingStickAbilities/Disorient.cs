using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/WalkingStickAbilities/Disorient")]
public class Disorient : Ability
{
    public override void Activate(Unit caster, Unit target)
    {

        List<Tuple<StatType, int, float>> debuff = new()
        {
            new Tuple<StatType, int, float>(StatType.Speed, caster.currentAggression, aggressionScaling)
        };

        target.ApplyStatusEffect(new StatusEffect(StatusEffectType.Debuff, effectDuration, debuff, false));

        int dmg = ApplyScaling(caster, caster.currentStrength, strengthScaling);
        
        target.TakeDamage(dmg);

        Debug.Log($"{caster.unitName} casts {abilityName} on {target.unitName}");
    }
}
