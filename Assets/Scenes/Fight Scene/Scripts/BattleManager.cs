using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private List<Unit> friendlyUnits = new List<Unit>();
    private List<Unit> enemyUnits = new List<Unit>();

    public List<Unit> allUnits => friendlyUnits.Concat(enemyUnits).ToList();
    public Queue<Unit> turnQueue = new Queue<Unit>();

    private Unit currentUnit;

    public static BattleManager Instance;
    private int battleStatus = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void InitializeBattle(List<Unit> friendlies, List<Unit> enemies)
    {
        friendlyUnits = friendlies;
        enemyUnits = enemies;

        StartCoroutine(BattleLoop());
    }

    IEnumerator BattleLoop()
    {
        while (true)
        {
            while (!turnQueue.Any())
            {
                foreach (var unit in allUnits)
                {
                    if (!unit.IsAlive()) continue;

                    unit.UpdateStatusEffects();

                    if (!unit.IsAlive()) continue;

                    unit.currentEnergy += unit.currentSpeed;
                    unit.bars.setEnergy(unit.currentEnergy);

                    yield return new WaitForSeconds(0.2f);

                    if (unit.currentEnergy >= 10 && !turnQueue.Contains(unit))
                    {
                        turnQueue.Enqueue(unit);
                    }
                }
                yield return new WaitForSeconds(0.2f);
            }

            SortTurnQueue();

            currentUnit = turnQueue.Dequeue();

            if (currentUnit.IsAlive())
            {
                yield return StartCoroutine(HandleTurn(currentUnit));
                currentUnit.TakeEnergy(10);
            }
        }
    }

    private void SortTurnQueue()
    {
        var sorted = turnQueue.OrderByDescending(u => u.currentEnergy).ToList();
        turnQueue.Clear();
        foreach (var unit in sorted)
        {
            turnQueue.Enqueue(unit);
        }
    }

    IEnumerator HandleTurn(Unit unit)
    {

        battleStatus = CheckEndConditions();

        if (battleStatus != 0)
        {
            EndBattle();
        }

        if (!unit.IsAlive()) yield break;

        foreach (var u in allUnits)
        {
            u.HighlightAsCurrentTurn(u == unit);
        }

        if (unit.isFriendly)
        {
            BattleUI.Instance.ShowActions(unit);
            yield return new WaitUntil(() => BattleUI.Instance.actionChosen);
        }
        else
        {
            if (unit.abilities == null || unit.abilities.Length == 0)
            {
                Debug.LogWarning($"{unit.unitName} has no abilities!");
                yield return new WaitForSeconds(0.4f);
                yield break;
            }

            var ability = unit.abilities[Random.Range(0, unit.abilities.Length)];

            // Get valid targets
            List<Unit> validTargets = GetValidTargetsForAbility(unit, ability);

            if (validTargets.Count > 0)
            {
                if (IsMultiTargetAbility(ability))
                {
                    // For multi-target abilities
                    foreach (var target in validTargets)
                    {
                        ability.Activate(unit, target);
                    }
                }
                else
                {
                    // For single-target abilities
                    var target = validTargets[Random.Range(0, validTargets.Count)];
                    ability.Activate(unit, target);
                }
            }
            else
            {
                Debug.LogWarning($"{unit.unitName} found no valid targets for {ability.abilityName}!");
            }

            yield return new WaitForSeconds(0.4f);
        }
    }

    public bool IsMultiTargetAbility(Ability ability)
    {
        return ability.canTarget == Ability.CanTarget.FrontlaneBoth ||
               ability.canTarget == Ability.CanTarget.BacklaneBoth;
    }

    public List<Unit> GetValidTargetsForAbility(Unit caster, Ability ability)
    {
        List<Unit> potentialTargets = new List<Unit>();

        bool targetsFriendly = ability.canTargetFriendly;
        bool isMultiTarget = IsMultiTargetAbility(ability);

        List<Unit> targetTeam = targetsFriendly
            ? (caster.isFriendly ? friendlyUnits : enemyUnits)
            : (caster.isFriendly ? enemyUnits : friendlyUnits);

        foreach (var unit in targetTeam)
        {
            if (!unit.IsAlive()) continue;

            switch (ability.canTarget)
            {
                case Ability.CanTarget.AnySingle:
                    potentialTargets.Add(unit);
                    break;

                case Ability.CanTarget.FrontlaneSingle:
                case Ability.CanTarget.FrontlaneBoth:
                    if (unit.isFrontlane)
                        potentialTargets.Add(unit);
                    break;

                case Ability.CanTarget.BacklaneSingle:
                case Ability.CanTarget.BacklaneBoth:
                    if (unit.isBacklane)
                        potentialTargets.Add(unit);
                    break;
            }
        }

        return potentialTargets;
    }

    public List<Unit> GetHighlightableTargets(Unit caster, Ability ability)
    {
        return GetValidTargetsForAbility(caster, ability);
    }


    // battle end status - 0 not over, 1 win, 2 defeat
    int CheckEndConditions()
    {

        bool allFriendlyUnitsDead = friendlyUnits.All(unit => !unit.IsAlive());
        bool allEnemyUnitsDead = enemyUnits.All(unit => !unit.IsAlive());

        if (allFriendlyUnitsDead)
        {
            return 2;
        }else if (allEnemyUnitsDead)
        {
            return 1;
        }

        return 0;
    }

    void EndBattle()
    {
        BattleUI.Instance.battleResultsPanel.SetActive(true);

        if (battleStatus == 1)
        {
            BattleUI.Instance.endText.text = "Victory!";
        } else
        {
            BattleUI.Instance.endText.text = "Defeat!";
        }
            
    }
}