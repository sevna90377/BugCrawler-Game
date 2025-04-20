using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public UnitData data;

    public int currentHP;
    public int energy;
    public bool isFriendly;
    public Bars bars;
    private Canvas barsCanvas;

    public void Init(UnitData unitData, bool friendly)
    {
        data = unitData;
        currentHP = data.maxHP;
        isFriendly = friendly;
        energy = 0;
        GetComponent<SpriteRenderer>().sprite = data.unitSprite;
        bars.SetMax(data.maxHP, 200);

        barsCanvas = bars.GetComponentInParent<Canvas>();

        if (!isFriendly) FlipBarsX();
    }

    public void TakeDamage(int damageTaken)
    {
        currentHP -= damageTaken;
        if (currentHP < 0) currentHP = 0;

        bars.setHealth(currentHP);
    }

    public void TakeEnergy(int energyTaken)
    {
        energy -= energyTaken;

        bars.setEnergy(energy);
    }

    public bool IsAlive() => currentHP > 0;

    private void OnMouseDown()
    {
        if (!BattleUI.Instance.waitingForTarget) return;

        var currentAbility = BattleUI.Instance.currentAbility;
        var currentCaster = BattleUI.Instance.currentUnit;

        if (!IsAlive()) return;

        bool sameTeam = isFriendly == currentCaster.isFriendly;

        if (sameTeam && currentAbility.canTargetFriendly)
        {
            BattleUI.selectedTarget = this;
        }
        else if (!sameTeam && !currentAbility.canTargetFriendly)
        {
            BattleUI.selectedTarget = this;
        }
    }

    private void FlipBarsX()
    {
        if (barsCanvas == null) return;

        Vector3 localPos = barsCanvas.transform.localPosition;
        localPos.x = -localPos.x;
        barsCanvas.transform.localPosition = localPos;
    }

}
