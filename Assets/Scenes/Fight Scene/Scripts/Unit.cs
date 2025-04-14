using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public UnitData data;

    public int currentHP;
    public int energy;
    public bool isFriendly;

    public void Init(UnitData unitData, bool friendly)
    {
        data = unitData;
        currentHP = data.maxHP;
        isFriendly = friendly;
        energy = 0;
        GetComponent<SpriteRenderer>().sprite = data.unitSprite;
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if (currentHP < 0) currentHP = 0;
    }

    public bool IsAlive() => currentHP > 0;

    private void OnMouseDown()
    {
        if (!BattleUI.Instance.waitingForTarget) return;

        var currentAbility = BattleUI.Instance.currentAbility;

        if (isFriendly && currentAbility.canTargetFriendly)
        {
            BattleUI.selectedTarget = this;
            Debug.Log("Selected friendly target: " + data.unitName);
        }
        else if (!isFriendly && !currentAbility.canTargetFriendly)
        {
            BattleUI.selectedTarget = this;
            Debug.Log("Selected enemy target: " + data.unitName);
        }
    }
}
