using UnityEngine;
using System.Collections.Generic;

public class BattleSetup : MonoBehaviour
{
    public Transform[] friendlyFrontSpawns;
    public Transform[] friendlyBackSpawns;
    public Transform[] enemyFrontSpawns;
    public Transform[] enemyBackSpawns;

    public BattleManager battleManager;
    public Unit unitPrefab;

    public List<UnitData> friendlyFrontlane;
    public List<UnitData> friendlyBacklane;
    public List<UnitData> enemyFrontlane;
    public List<UnitData> enemyBacklane;

    void Start()
    {
        List<Unit> spawnedFriendlies = new List<Unit>();
        List<Unit> spawnedEnemies = new List<Unit>();

        // Spawn friendlies
        spawnedFriendlies.AddRange(SpawnUnits(friendlyFrontlane, friendlyFrontSpawns));
        spawnedFriendlies.AddRange(SpawnUnits(friendlyBacklane, friendlyBackSpawns));

        // Spawn enemies
        spawnedEnemies.AddRange(SpawnUnits(enemyFrontlane, enemyFrontSpawns));
        spawnedEnemies.AddRange(SpawnUnits(enemyBacklane, enemyBackSpawns));

        battleManager.InitializeBattle(spawnedFriendlies, spawnedEnemies);

        Debug.Log("Battle initialized with lanes.");
    }

    List<Unit> SpawnUnits(List<UnitData> unitsData, Transform[] spawnPoints)
    {
        List<Unit> spawnedUnits = new List<Unit>();

        for (int i = 0; i < unitsData.Count && i < spawnPoints.Length; i++)
        {
            var unitGO = Instantiate(unitPrefab.gameObject, spawnPoints[i].position, Quaternion.identity);
            unitGO.name = unitsData[i].unitName;
            var unit = unitGO.GetComponent<Unit>();

            unit.Init(unitsData[i]);

            if (unitsData[i].unitSprite != null)
            {
                unit.GetComponent<SpriteRenderer>().sprite = unitsData[i].unitSprite;
            }

            spawnedUnits.Add(unit);
        }

        return spawnedUnits;
    }
}