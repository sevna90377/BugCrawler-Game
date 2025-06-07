using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Cubes;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapExtensions
{
    public static void SelectCubes(this Tilemap tilemap, IList<Hex> hexes)
    {
        if (hexes != null)
        {
            foreach (var hex in hexes)
            {
                var vector3Int = Hex.QoffsetFromCube(hex);

                var tile = tilemap.GetTile<SampleTile>(vector3Int);

                if (tile)
                {
                    tile.SetSelect(true);
                    tile.RefreshTile(vector3Int, tilemap);
                }
            }
        }
    }

    public static void DeselectTiles(this Tilemap tilemap)
    {
        BoundsInt bounds = tilemap.cellBounds;
        var tiles = tilemap.GetTilesBlock(bounds).Cast<SampleTile>();

        foreach (var tile in tiles)
        {
            if (tile)
            {
                tile.SetSelect(false);
            }
        }

        tilemap.RefreshAllTiles();
    }
}
