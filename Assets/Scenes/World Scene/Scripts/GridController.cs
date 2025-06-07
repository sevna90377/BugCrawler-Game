using System.Linq;
using Assets.Scripts.Cubes;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

/// <summary>
/// https://www.redblobgames.com/grids/hexagons/
///
/// Unity is YXZ!!! odd-q -> shoves odd columns by +½ row
/// 
/// </summary>
public class GridController : MonoBehaviour
{
    private Grid _grid;

    private HexGrid _hexGrid;

    private Hex _startHex;

    [SerializeField] 
    private Tilemap interactiveMap;

    [SerializeField]
    private GameObject debugPosition;

    // Start is called before the first frame update
    void Start()
    {
        _grid = gameObject.GetComponent<Grid>();

        _hexGrid = new HexGrid(interactiveMap);

        //highlightMap.SetTile(new Vector3Int(-2,-2,0), highlightTile)

        // Debug
        //BoundsInt bounds = interactiveMap.cellBounds;
        //for (int x = bounds.xMin; x <= bounds.xMax; x++)
        //{
        //    for (int y = bounds.yMin; y <= bounds.yMax; y++)
        //    {
        //        Vector3Int cellPosition = new Vector3Int(x, y, 0);
        //        if (interactiveMap.HasTile(cellPosition))
        //        {
        //            //var genericTile = interactiveMap.GetTile<GenericTile>(cellPosition);

        //            Vector3 worldPosition = interactiveMap.CellToWorld(cellPosition);

        //            GameObject obj = Instantiate(debugPosition);

        //            var c = obj.GetComponent<TileWithTextBehaviour>();

        //            c.setText(cellPosition.ToString());
        //            //c.setText(Hex.QoffsetToCube(cellPosition).ToString());

        //            obj.transform.rotation = interactiveMap.transform.rotation;
        //            obj.transform.position = new Vector3(worldPosition.x, worldPosition.y, -1);
        //        }
        //    }
        //}
    }

    // Update is called once per frame
    void Update()
    {
        var mousePosition = GetMousePosition();
        mousePosition = new Vector3Int(mousePosition.x, mousePosition.y, 0);

        Debug.Log("update:" + mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            interactiveMap.DeselectTiles();

            _startHex = null;

            var hex = Hex.QoffsetToCube(mousePosition);
            var hexes = hex.GetAdjacent();
            hexes.Add(hex);
            interactiveMap.SelectCubes(hexes);
        }

        if (Input.GetMouseButtonDown(1))
        {
            _startHex = Hex.QoffsetToCube(mousePosition);
        }

        if (_startHex != null)
        {
            interactiveMap.DeselectTiles();

            // GetLine
            //interactiveMap.SelectCubes(_startHex.GetLine(Hex.QoffsetToCube(mousePosition)));

            // ReachableHexes
            //interactiveMap.SelectCubes(_startHex.ReachableHexes(5, _hexGrid));
            //_startHex = null;

            // CalculateVisibleHexes
            //interactiveMap.SelectCubes(_startHex.CalculateVisibleHexes(5, _hexGrid).ToList());
            //_startHex = null;

            // FindPath
            interactiveMap.SelectCubes(_startHex.FindPath(Hex.QoffsetToCube(mousePosition), _hexGrid));
        }
    }

    private Vector3Int GetMousePosition()
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0f;
        return _grid.WorldToCell(position);
    }
}
