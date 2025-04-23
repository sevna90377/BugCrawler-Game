using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SampleTile : Tile
{
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = GetSprite(position);
    }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }

    [Header("Tile block")]
    public Vector2Int m_size = Vector2Int.one;
    public Sprite[] m_Sprites;

    public Sprite GetSprite(Vector3Int pos)
    {
        if (m_Sprites.Length != m_size.x * m_size.y) return sprite;

        while (pos.x < m_size.x) { pos.x += m_size.x; }
        while (pos.y < m_size.y) { pos.y += m_size.y; }

        int x = pos.x;
        int y = pos.y;

        int index = x + (((m_size.y - 1) * m_size.x) - y * m_size.x);

        return m_Sprites[0];
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
