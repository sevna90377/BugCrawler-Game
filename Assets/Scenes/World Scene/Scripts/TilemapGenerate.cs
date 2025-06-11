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

    [Header("Points of Interest")]
    [SerializeField] private int chestDensity = 20; // lower = more chests


    public static List<Vector3Int> obstacleList = new();

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

                // Mark core room
                List<Vector2Int> baseTiles = new();
                for (int i = newRoom.xMin; i < newRoom.xMax; i++)
                {
                    for (int j = newRoom.yMin; j < newRoom.yMax; j++)
                    {
                        roomMask[i, j] = true;
                        baseTiles.Add(new Vector2Int(i, j));
                    }
                }

                // Add extra tiles
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

        ConnectRooms(placedRooms, roomMask);

        // Set entry/exit based on first/last room
        if (placedRooms.Count > 0)
        {
            RectInt firstRoom = placedRooms[0];
            RectInt lastRoom = placedRooms[placedRooms.Count - 1];

            SampleTile.EntryPosition = new Vector3Int(
                firstRoom.yMin + firstRoom.height / 2,
                firstRoom.xMin + firstRoom.width / 2,
                0
            );

            SampleTile.ExitPosition = new Vector3Int(
                lastRoom.yMin + lastRoom.height / 2,
                lastRoom.xMin + lastRoom.width / 2,
                0
            );

            Debug.Log($"Set entry at {SampleTile.EntryPosition}, exit at {SampleTile.ExitPosition}");
        }


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
                    obstacleList.Add(cell);
                    tile.obstacles.Add(cell);
                }

                tilemap.SetTile(cell, tile);
            }
        }

        PlacePointsOfInterest(placedRooms, roomMask);


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

        var rng = new System.Random();

        for (int i = 0; i < centers.Count - 1; i++)
        {
            Vector2Int from = centers[i];
            Vector2Int to = centers[i + 1];

            Vector2Int current = from;

            // Randomized fuzzy shortest path using shuffled direction steps
            List<Vector2Int> path = new();

            while (current != to)
            {
                List<Vector2Int> possibleSteps = new();

                if (current.x < to.x) possibleSteps.Add(Vector2Int.right);
                if (current.x > to.x) possibleSteps.Add(Vector2Int.left);
                if (current.y < to.y) possibleSteps.Add(Vector2Int.up);
                if (current.y > to.y) possibleSteps.Add(Vector2Int.down);

                for (int s = possibleSteps.Count - 1; s > 0; s--)
                {
                    int j = rng.Next(s + 1);
                    (possibleSteps[s], possibleSteps[j]) = (possibleSteps[j], possibleSteps[s]);
                }

                if (possibleSteps.Count > 0)
                    current += possibleSteps[0];

                if (current.x >= 0 && current.x < roomMask.GetLength(0) &&
                    current.y >= 0 && current.y < roomMask.GetLength(1))
                {
                    roomMask[current.x, current.y] = true;
                }
            }
        }
    }

    private void PlacePointsOfInterest(List<RectInt> rooms, bool[,] roomMask)
    {
        var rng = new System.Random();

        List<Vector3Int> floorTiles = new();

        SampleTile.enemyCamps.Clear();
        SampleTile.chests.Clear();

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (!roomMask[x, y]) continue;

                var pos = new Vector3Int(y, x, 0); 
                if (pos == SampleTile.EntryPosition || pos == SampleTile.ExitPosition)
                    continue;

                floorTiles.Add(pos);
            }
        }

        HashSet<Vector3Int> used = new();

        // Place enemy camps: 1 per room, avoid center
        foreach (var room in rooms)
        {
            List<Vector3Int> roomFloor = new();

            for (int x = room.xMin; x < room.xMax; x++)
            {
                for (int y = room.yMin; y < room.yMax; y++)
                {
                    var pos = new Vector3Int(y, x, 0); // YXZ swap for Unity grid

                    if (pos == SampleTile.EntryPosition || pos == SampleTile.ExitPosition)
                        continue;

                    if (tilemap.HasTile(pos))
                        roomFloor.Add(pos);
                }
            }

            if (roomFloor.Count > 0)
            {
                var chosen = roomFloor[rng.Next(roomFloor.Count)];

                SampleTile.enemyCamps.Add(chosen);
                tilemap.RefreshTile(chosen);
                used.Add(chosen);
            }
        }


        // Place chests: floorTiles.Count / chestDensity
        int chestCount = Mathf.FloorToInt(floorTiles.Count / (float)Mathf.Max(1, chestDensity));
        floorTiles.Shuffle();

        int placed = 0;
        foreach (var pos in floorTiles)
        {
            if (used.Contains(pos)) continue;

            SampleTile.chests.Add(pos);
            tilemap.RefreshTile(pos);
            used.Add(pos);
            placed++;

            if (placed >= chestCount) break;
        }

        Debug.Log($"Placed {SampleTile.enemyCamps.Count} enemy camps and {placed} chests.");
    }




}
