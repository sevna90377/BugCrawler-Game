using UnityEngine;
using System.Collections.Generic;

public class BattleSetup : MonoBehaviour
{
    public GameObject unitPrefab;

    public Transform[] friendlySpawns;
    public Transform[] enemySpawns;

    public UnitData[] friendlyUnitData;
    public UnitData[] enemyUnitData;

    public BattleManager battleManager;

    void Start()
    {
        SpawnUnits();
        Debug.Log("Spawning units...");
    }

    void SpawnUnits()
    {
        battleManager.friendlyUnits = new List<Unit>();
        battleManager.enemyUnits = new List<Unit>();

        // Friendly
        for (int i = 0; i < friendlySpawns.Length; i++)
        {
            var unitGO = Instantiate(unitPrefab, friendlySpawns[i].position, Quaternion.identity);
            var unit = unitGO.GetComponent<Unit>();
            unit.Init(friendlyUnitData[i], true);

            battleManager.friendlyUnits.Add(unit);
        }

        // Enemy
        for (int i = 0; i < enemySpawns.Length; i++)
        {
            var unitGO = Instantiate(unitPrefab, enemySpawns[i].position, Quaternion.identity);
            var unit = unitGO.GetComponent<Unit>();
            unit.Init(enemyUnitData[i], false);

            battleManager.enemyUnits.Add(unit);
        }
    }
}
