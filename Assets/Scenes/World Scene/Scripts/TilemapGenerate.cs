using System;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TilemapGenerate : MonoBehaviour
{
    public Tilemap tilemap; // Assign in the Inspector
    public TileBase[] aaa;
    

    // Start is called before the first frame update
    void Start()
    {
        tilemap = (Tilemap)FindAnyObjectByType(typeof(Tilemap));

        GenerateMap();

    }

    public void GenerateMap()
    {
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 25; j++)
            {
                tilemap.SetTile(new Vector3Int(i, j, 0), aaa[0]);
            }
        }
    }

}
