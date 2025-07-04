using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/AntWarriorAbilities/SpearThrust")]
public class SpearThrust : Ability
{
    public override void Activate(Unit caster, Unit target)
    {
        int damage = ApplyScaling(caster, caster.currentStrength, strengthScaling);
        target.TakeDamage(damage);

        Debug.Log($"{caster.unitName} casts {abilityName} on {target.unitName}");

    }
}
