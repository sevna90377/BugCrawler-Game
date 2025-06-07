using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGenerate : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int mapWidth = 15;
    [SerializeField] private int mapHeight = 10;

    [Header("Tilemap and Tiles")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private SampleTile sampleTilePrefab; 

    [Header("Room Settings")]
    [SerializeField] private int roomCount = 3;
    [SerializeField] private Vector2Int roomSize = new(4, 4);

    private void Start()
    {
        if (tilemap == null)
            tilemap = FindAnyObjectByType<Tilemap>();

        GenerateMap();
    }

    private void GenerateMap()
    {
        bool[,] roomMask = new bool[mapWidth, mapHeight];
        var rng = new System.Random();
        List<RectInt> placedRooms = new();

        int attempts = 0;

        while (placedRooms.Count < roomCount && attempts < 100)
        {
            int x = rng.Next(0, mapWidth - roomSize.x);
            int y = rng.Next(0, mapHeight - roomSize.y);
            var newRoom = new RectInt(x, y, roomSize.x, roomSize.y);

            bool overlaps = false;
            foreach (var room in placedRooms)
            {
                if (newRoom.Overlaps(room))
                {
                    overlaps = true;
                    break;
                }
            }

            if (!overlaps)
            {
                placedRooms.Add(newRoom);

                for (int i = newRoom.xMin; i < newRoom.xMax; i++)
                {
                    for (int j = newRoom.yMin; j < newRoom.yMax; j++)
                    {
                        roomMask[i, j] = true;
                    }
                }
            }

            attempts++;
        }

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                var cell = new Vector3Int(y, x, 0);

                var tile = ScriptableObject.CreateInstance<SampleTile>();

                tile.sprites = sampleTilePrefab.sprites;
                tile.size = sampleTilePrefab.size;

                if (!roomMask[x, y])
                {
                    tile.obstacles.Add(cell);
                }

                tilemap.SetTile(cell, tile);
            }
        }

        Debug.Log($"Generated {placedRooms.Count} rooms on a {mapWidth}x{mapHeight} grid using SampleTile.");
    }
}
