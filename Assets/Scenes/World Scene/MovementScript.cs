using System.Collections.Generic;
using Assets.Scripts.Cubes;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MovementScript : MonoBehaviour
{
    public Tilemap tilemap;
    public float hexSize = 1f;

    private bool loaded = false;
    private HexGrid _hexGrid;

    // Start is called before the first frame update
    void Start()
    {
        //SwitchMap();
        transform.position = tilemap.CellToWorld(new Vector3Int(0,0,0));
    }

    // Update is called once per frame
    void Update()
    {
        if (!loaded)
        {
            transform.position = tilemap.CellToWorld(SampleTile.EntryPosition);
            loaded = true;
        }
        if (Input.GetMouseButtonDown(0))
        {
            HandleInput();
        }
        
    }

    void HandleInput()
    {
        Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        clickPos.z = 0;

        Vector3Int clickCell = tilemap.WorldToCell(clickPos);
        Hex clickHex = Hex.QoffsetToCube(clickCell);

        Vector3Int playerCell = tilemap.WorldToCell(transform.position);
        Hex playerHex = Hex.QoffsetToCube(playerCell);

        if (_hexGrid == null)
        {
            _hexGrid = new HexGrid(tilemap);
        }

        IList<Hex> reachable = playerHex.ReachableHexes(3, _hexGrid);

        if (tilemap.HasTile(clickCell))
        {
            if (reachable.Contains(clickHex) && playerHex.FindPath(clickHex, _hexGrid).Count <= 4 && !TilemapGenerate.obstacleList.Contains(clickCell))
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
        transform.position = tilemap.GetCellCenterWorld(targetCell);

        if (SampleTile.enemyCamps.Contains(targetCell))
        {
            SampleTile.enemyCamps.Remove(targetCell);
            SampleTile.visitedCamps.Add(targetCell);
            tilemap.RefreshTile(targetCell);

            Debug.Log($"Visited enemy camp at {targetCell}");
        }
        
        if (SampleTile.chests.Contains(targetCell))
        {
            SampleTile.chests.Remove(targetCell);
            SampleTile.visitedChests.Add(targetCell);
            tilemap.RefreshTile(targetCell);

            Debug.Log($"Visited chest at {targetCell}");
        }
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
     * unity has (x,y) (written as (y,x,z)) hexagonal coordinates where x values grow to the right, and y values grow upwards
     * that brings issues on diagonals, as x axis would change every 2 steps
     * that's why we have different adjacency definition on even and odd cells
     */
/*    bool CheckAdjacency(Vector3Int targetCell)
    {
        Vector3Int playerCell = tilemap.WorldToCell(transform.position);

        foreach (Vector3Int direction in (playerCell.y % 2 == 0 ? adjacent_even : adjacent_odd))
        {
            Vector3Int adjacentCell = playerCell + direction;

            if (targetCell == adjacentCell)
            {
                return true;
            }
        }
        return false;
    }*/
    ///

    ///
    /// MAP SWITCHING GENERAL LOGIC
    ///
    // public GameObject[] tilemapPrefabs;
    // public GameObject currentMap;
    // int map = 0;
    /*
     * just store the prefabs in an array and randomize
     * could be used for map generation by offsets and matching prefabs for different directions
     * since the prefab is whole grid, multiple tilemap layers can be used for map objects
     */
 /*   void SwitchMap()
    {
        if (currentMap != null)
            Destroy(currentMap);
        currentMap = Instantiate(tilemapPrefabs[map], Vector3.zero, Quaternion.identity);
        tilemap = currentMap.GetComponentInChildren<Tilemap>();
        map = (map + 1) % (tilemapPrefabs.Length);
    }*/
    ///
}
