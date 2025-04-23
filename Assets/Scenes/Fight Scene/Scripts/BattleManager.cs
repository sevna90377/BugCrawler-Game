using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public List<Unit> friendlyUnits;
    public List<Unit> enemyUnits;

    public List<Unit> allUnits => friendlyUnits.Concat(enemyUnits).ToList();
    public Queue<Unit> turnQueue = new Queue<Unit>();

    private Unit currentUnit;

    void Start()
    {
        StartCoroutine(BattleLoop());
    }

    IEnumerator BattleLoop()
    {
        while (true)
        {
            // Fill energy until one or more units can act
            while (!turnQueue.Any())
            {
                foreach (var unit in allUnits)
                {
                    if (!unit.IsAlive()) continue;

                    unit.energy += unit.data.energyGain;
                    if (unit.energy >= 100)
                    {
                        unit.TakeEnergy(100);
                        turnQueue.Enqueue(unit);
                    }
                }
                yield return new WaitForSeconds(0.2f);
            }

            currentUnit = turnQueue.Dequeue();
            yield return StartCoroutine(HandleTurn(currentUnit));
        }
    }

    IEnumerator HandleTurn(Unit unit)
    {
        if (!unit.IsAlive()) yield break;

        if (unit.isFriendly)
        {
            // Player unit — show UI and wait for input
            BattleUI.Instance.ShowActions(unit);
            yield return new WaitUntil(() => BattleUI.Instance.actionChosen);
        }
        else
        {
            // Defensive check for empty or null ability list
            if (unit.data.abilities == null || unit.data.abilities.Length == 0)
            {
                Debug.LogWarning($"{unit.data.unitName} has no abilities!");
                yield return new WaitForSeconds(0.4f);
                yield break;
            }

            // Choose a truly random ability from this unit's list
            int randomIndex = Random.Range(0, unit.data.abilities.Length);
            var ability = unit.data.abilities[randomIndex];

            // Choose valid targets based on ability type
            List<Unit> potentialTargets = ability.canTargetFriendly
                ? enemyUnits.Where(u => u.IsAlive()).ToList()
                : friendlyUnits.Where(u => u.IsAlive()).ToList();

            if (potentialTargets.Count > 0)
            {
                var target = potentialTargets[Random.Range(0, potentialTargets.Count)];
                ability.Activate(unit, target);
            }
            else
            {
                Debug.LogWarning($"{unit.data.unitName} found no valid targets for {ability.abilityName}!");
            }

            yield return new WaitForSeconds(0.4f);
        }
    }

}
