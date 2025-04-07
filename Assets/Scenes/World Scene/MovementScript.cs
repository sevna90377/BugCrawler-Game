using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class MovementScript : MonoBehaviour
{
    public Tilemap tilemap;
    public float hexSize = 1f;

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

    // Start is called before the first frame update
    void Start()
    {
        transform.position = tilemap.CellToWorld(new Vector3Int(1,1,0));
    }

    // Update is called once per frame
    void Update()
    {
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

        if (tilemap.HasTile(clickCell))
        {
            Move(clickCell);
        }
        else
        {
            Debug.Log("No tile at clicked position.");
        }
    }

    void Move(Vector3Int targetCell)
    {
        Vector3Int playerCell = tilemap.WorldToCell(transform.position);

        foreach (Vector3Int direction in (playerCell.y % 2 == 0 ? adjacent_even : adjacent_odd))
        {
            Vector3Int adjacentCell = playerCell + direction;

            if (targetCell == adjacentCell)
            {
                Debug.Log("Clicked tile is adjacent!");
            }
        }
    }
}
