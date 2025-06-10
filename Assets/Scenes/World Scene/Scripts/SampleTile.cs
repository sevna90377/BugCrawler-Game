using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "2D/Custom Tiles/Variable Tile")]
public class SampleTile : Tile
{
    /// <summary>
    /// If true - Tile is an obstacle impossible to pass.
    /// </summary>
    public List<Vector3Int> obstacles = new();

    public Color selectedColor = Color.gray;

    public Sprite[] sprites;

    public Vector2Int size = Vector2Int.one;


    // TODO: Find a good lerp factor 
    private float lerpFactor = 0.75f;

    private bool _selected;

    public static Vector3Int EntryPosition = new(-999, -999, 0);
    public static Vector3Int ExitPosition = new(-999, -999, 0);

    public static List<Vector3Int> enemyCamps = new();
    public static List<Vector3Int> visitedCamps = new();
    public static List<Vector3Int> chests = new();
    public static List<Vector3Int> visitedChests = new();


    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);

        tileData.sprite = GetSprite(position);

        if (obstacles.Contains(position))
        {
            tileData.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        }
        else if (position == EntryPosition)
        {
            tileData.color = Color.green;
        }
        else if (position == ExitPosition)
        {
            tileData.color = Color.yellow;
        }
        else if (enemyCamps.Contains(position))
        {
            tileData.color = Color.red;
        }
        else if (visitedCamps.Contains(position))
        {
            tileData.color = new Color(0.5803922f, 0f, 0.8274511f, 1f);
        }
        else if (chests.Contains(position))
        {
            tileData.color = Color.cyan;
        }
        else if (visitedChests.Contains(position))
        {
            tileData.color = Color.blue;
        }
         
        if (_selected && !obstacles.Contains(position))
        {
            tileData.color = Color.Lerp(Color.cyan, tileData.color, lerpFactor);
        }

/*
        if (position == EntryPosition)
        {
            tileData.color = Color.green;
        }
        else if (position == ExitPosition)
        {
            tileData.color = Color.yellow;
        }
        else if(obstacles.Contains(position))
        {
            tileData.color = Color.red;
        }
        else if (_selected)
        {
            tileData.color = Color.black; //selectedColor;
        }
*/
    }

    public void SetSelect(bool selected)
    {
        _selected = selected;
    }

    public Sprite GetSprite(Vector3Int pos)
    {
        if (sprites.Length != size.x * size.y) return sprite;

        while (pos.x < 0) { pos.x += size.x; }
        while (pos.y < 0) { pos.y += size.y; }

        int x = pos.x % size.x;
        int y = pos.y % size.y;
        int index = x + (((size.y - 1) * size.x) - y * size.x);

        return sprites[index]; // <-- fixed
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/2D/Custom Tiles/Variable Tile")]
    public static void CreateVariableTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Variable Tile", "New Variable Tile", "Asset", "Save Variable Tile", "Assets");
        if (path == "") return;

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SampleTile>(), path);
    }
#endif
}
