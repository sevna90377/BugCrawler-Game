using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/MosquitoAbilities/InjectPoison")]
public class InjectPoison : Ability
{
    public override void Activate(Unit caster, Unit target)
    {

        int dmg = ApplyScaling(caster, caster.currentStrength, strengthScaling);
        target.TakeDamage(dmg);

        target.ApplyStatusEffect(new StatusEffect(StatusEffectType.Poison, effectDuration, caster.currentAggression, aggressionScaling));

        List<Tuple<StatType, int, float>> debuff = new()
        {
            new Tuple<StatType, int, float>(StatType.Speed, caster.currentAggression, aggressionScaling)
        };

        caster.ApplyStatusEffect(new StatusEffect(StatusEffectType.Debuff, effectDuration, debuff, false));

        Debug.Log($"{caster.unitName} casts {abilityName} on {target.unitName}");
    }
}
