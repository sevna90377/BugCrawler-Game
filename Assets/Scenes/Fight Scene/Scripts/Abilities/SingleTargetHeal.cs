using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/SingleTargetHeal")]
public class SingleTargetHeal : Ability
{
    public int healAmount;

    public override void Activate(Unit user, Unit target)
    {
        target.currentHP += healAmount;
        if (target.currentHP > target.data.maxHP)
            target.currentHP = target.data.maxHP;

        Debug.Log($"{user.data.unitName} casts {abilityName} on {target.data.unitName}, healing {healAmount} HP!");
    }
}