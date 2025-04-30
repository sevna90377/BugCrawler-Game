using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/SingleTargetHeal")]
public class SingleTargetHeal : Ability
{
    public int healAmount;

    public override void Activate(Unit user, Unit target)
    {
        int trueHeal;
        if (target.currentHP + healAmount > target.maxHP)
        {
            trueHeal = target.maxHP - target.currentHP;
            target.TakeDamage(-trueHeal);
        }
        else
        {
            trueHeal = healAmount;
            target.TakeDamage(-trueHeal);
        }

        Debug.Log($"{user.unitName} casts {abilityName} on {target.unitName}, healing {trueHeal} HP!");
    }
}