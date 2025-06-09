using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/AntWarriorAbilities/BraceShield")]
public class BraceShield : Ability
{
    public override void Activate(Unit caster, Unit target)
    {
        int healAmount = ApplyScaling(caster, caster.currentPower, powerScaling);
        target.Heal(healAmount);
        caster.Heal(healAmount);

        Debug.Log($"{caster.unitName} casts {abilityName} on {target.unitName}");
    }
}
