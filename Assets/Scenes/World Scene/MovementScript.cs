using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class MovementScript : MonoBehaviour
{

    Tilemap walkable;
    public float hexSize = 1f;

    // Start is called before the first frame update
    void Start()
    {
        SwitchMap();
        walkable = currentMap.transform.Find("Walkable").GetComponent<Tilemap>();
        transform.position = walkable.CellToWorld(new Vector3Int(0,0,0));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleInput();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchMap();
        }
    }

    void HandleInput()
    {
        Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        clickPos.z = 0;
        Vector3Int clickCell = walkable.WorldToCell(clickPos);

        if (walkable.HasTile(clickCell))
        {
            if (CheckAdjacency(clickCell))
            {
                Move(clickCell);
            }
        }
        else
        {
            Debug.Log("No tile at clicked position.");
        }
    }

    void Move(Vector3Int targetCell)
    {
        transform.position = walkable.GetCellCenterWorld(targetCell);
    }


    ///
    /// ADJACENCY ON HEXAGONAL GRID
    /// 
    Vector3Int[] adjacent_even = new Vector3Int[] {
        new Vector3Int(1, 0, 0),    // North
        new Vector3Int(-1, 0, 0),   // South
        new Vector3Int(0, -1, 0),   // N-West
        new Vector3Int(0, 1, 0),    // N-East
        new Vector3Int(-1, -1, 0),   // S-West
        new Vector3Int(-1, 1, 0)   // S-East
    };
    Vector3Int[] adjacent_odd = new Vector3Int[] {
        new Vector3Int(1, 0, 0),    // North
        new Vector3Int(-1, 0, 0),   // South
        new Vector3Int(1, -1, 0),   // N-West
        new Vector3Int(1, 1, 0),    // N-East
        new Vector3Int(0, -1, 0),   // S-West
        new Vector3Int(0, 1, 0)   // S-East
    };
    /*
     * unity has (x,y) hexagonal coordinates where x values grow to the right, and y values grow upwards
     * that brings issues on diagonals, as x axis would change every 2 steps
     * that's why we have different adjacency definition on even and odd cells
     */
    bool CheckAdjacency(Vector3Int targetCell)
    {
        Vector3Int playerCell = walkable.WorldToCell(transform.position);

        foreach (Vector3Int direction in (playerCell.y % 2 == 0 ? adjacent_even : adjacent_odd))
        {
            Vector3Int adjacentCell = playerCell + direction;

            if (targetCell == adjacentCell)
            {
                return true;
            }
        }
        return false;
    }
    ///

    ///
    /// MAP SWITCHING GENERAL LOGIC
    ///
    public GameObject[] maps;
    public GameObject currentMap;
    int map = 0;
    /*
     * just store the prefabs in an array and randomize
     * could be used for map generation by offsets and matching prefabs for different directions
     * since the prefab is whole grid, multiple tilemap layers can be used for map objects
     */
    void SwitchMap()
    {
        if (currentMap != null)
            Destroy(currentMap);
        currentMap = Instantiate(maps[map], Vector3.zero, Quaternion.identity);
        walkable = currentMap.GetComponentInChildren<Tilemap>();
        map = (map + 1) % (maps.Length);
    }
    ///
}
