using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/AntWarriorAbilities/Charge")]
public class Charge : Ability
{
    public override void Activate(Unit caster, Unit target)
    {
        int damage = ApplyScaling(caster, caster.currentStrength, strengthScaling);
        target.TakeDamage(damage);
        caster.TakeDamage(damage / 2);

        Debug.Log($"{caster.unitName} casts {abilityName} on {target.unitName}");
    }
}
