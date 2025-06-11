using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Cubes
{
    public class HexGrid : IHexGrid
    {
        private readonly Tilemap _tilemap;

        private List<Vector3Int> _obstacles;

        public HexGrid(Tilemap tilemap)
        {
            _tilemap = tilemap;
        }
        public bool IsValid(Hex hex)
        {
            Vector3Int position = Hex.QoffsetFromCube(hex);

            return _tilemap.GetTile(position) != null;
        }

        public bool IsObstacle(Hex hex)
        {
            Vector3Int position = Hex.QoffsetFromCube(hex);

            if (_obstacles == null || _obstacles.Count == 0)
            {
                var tile = _tilemap.GetTile<SampleTile>(position);

                if (tile != null)
                {
                    _obstacles = tile.obstacles;
                }
            }

            return _obstacles != null && _obstacles.Contains(position);
        }
    }
}