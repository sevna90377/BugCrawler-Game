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
                        unit.energy -= 100;
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
            // Show UI, wait for player input
            BattleUI.Instance.ShowActions(unit);
            yield return new WaitUntil(() => BattleUI.Instance.actionChosen);
        }
        else
        {
            // Simple AI (random ability & target)
            var ability = unit.data.abilities[Random.Range(0, 4)];
            var targets = friendlyUnits.Where(u => u.IsAlive()).ToList();
            var target = targets[Random.Range(0, targets.Count)];

            ability.Activate(unit, target);
            yield return new WaitForSeconds(1f);
        }
    }
}
