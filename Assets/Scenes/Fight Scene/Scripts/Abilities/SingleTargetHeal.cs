using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/SingleTargetHeal")]
public class SingleTargetHeal : Ability
{
    public int healAmount;

    public override void Activate(Unit user, Unit target)
    {
        int trueHeal;
        if (target.currentHP + healAmount > target.data.maxHP)
        {
            trueHeal = target.data.maxHP - target.currentHP;
            target.TakeDamage(-trueHeal);
        }
        else
        {
            trueHeal = healAmount;
            target.TakeDamage(-trueHeal);
        }

        Debug.Log($"{user.data.unitName} casts {abilityName} on {target.data.unitName}, healing {trueHeal} HP!");
    }
}