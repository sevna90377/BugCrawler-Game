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

    [Header("Room Extras")]
    [SerializeField] private int extraTilesPerRoom = 3;

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

                // Mark core rectangular room
                List<Vector2Int> baseTiles = new();
                for (int i = newRoom.xMin; i < newRoom.xMax; i++)
                {
                    for (int j = newRoom.yMin; j < newRoom.yMax; j++)
                    {
                        roomMask[i, j] = true;
                        baseTiles.Add(new Vector2Int(i, j));
                    }
                }

                // Add extra glued-on tiles
                int placedExtras = 0;
                Vector2Int[] directions = new Vector2Int[]
                {
                    new(1, 0), new(-1, 0),
                    new(0, 1), new(0, -1),
                    new(1, 1), new(-1, -1),
                    new(1, -1), new(-1, 1)
                };

                while (placedExtras < extraTilesPerRoom && baseTiles.Count > 0)
                {
                    var baseTile = baseTiles[rng.Next(baseTiles.Count)];
                    var offset = directions[rng.Next(directions.Length)];
                    var extra = baseTile + offset;

                    if (extra.x >= 0 && extra.y >= 0 &&
                        extra.x < mapWidth && extra.y < mapHeight &&
                        !roomMask[extra.x, extra.y])
                    {
                        roomMask[extra.x, extra.y] = true;
                        baseTiles.Add(extra); // Allow clusters
                        placedExtras++;
                    }
                }
            }

            attempts++;
        }

        // Done placing rooms and adding extras
        ConnectRooms(placedRooms, roomMask);

        // Paint tiles
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                var cell = new Vector3Int(y, x, 0); // YXZ swap for Unity grid

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

        Debug.Log($"Generated {placedRooms.Count} rooms (with extras) on a {mapWidth}x{mapHeight} grid using SampleTile.");
    }

    private void ConnectRooms(List<RectInt> rooms, bool[,] roomMask)
    {
        List<Vector2Int> centers = new();
        foreach (var room in rooms)
        {
            var center = new Vector2Int(
                room.xMin + room.width / 2,
                room.yMin + room.height / 2
            );
            centers.Add(center);
        }

        for (int i = 0; i < centers.Count - 1; i++)
        {
            Vector2Int from = centers[i];
            Vector2Int to = centers[i + 1];

            // Connect horizontally first, then vertically
            Vector2Int current = from;

            while (current.x != to.x)
            {
                roomMask[current.x, current.y] = true;
                current.x += (to.x > current.x) ? 1 : -1;
            }

            while (current.y != to.y)
            {
                roomMask[current.x, current.y] = true;
                current.y += (to.y > current.y) ? 1 : -1;
            }
        }
    }


}
