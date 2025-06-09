using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/MosquitoAbilities/Stab")]
public class Stab : Ability
{
    public override void Activate(Unit caster, Unit target)
    {

        int dmg = ApplyScaling(caster, caster.currentStrength, strengthScaling);
        target.TakeDamage(dmg);

        Debug.Log($"{caster.unitName} casts {abilityName} on {target.unitName}");
    }
}
