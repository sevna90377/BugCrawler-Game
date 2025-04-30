using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/BeeAbilities/Stickiness")]
public class Stickiness : Ability
{
    public override void Activate(Unit user, Unit target)
    {
        int damage = ApplyScaling(user, user.currentStrength, strengthScaling);
        target.TakeDamage(damage);

        List<Tuple<StatType, int, float>> debuff = new()
        { 
            new Tuple<StatType, int, float>(StatType.Speed, user.currentAggression, aggressionScaling) 
        };

        target.ApplyStatusEffect(new StatusEffect(StatusEffectType.Debuff, effectDuration, debuff, false));

        Debug.Log($"{user.unitName} casts {abilityName} on {target.unitName}");
    }
}
