using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

// Class that handles the generation of the match-3 board.
public class GridGenerator : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase[] tiles;

    private Vector3Int gridSize;

    private void Start()
    {
        tilemap.CompressBounds();
        gridSize = tilemap.size;
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        // Fills up the tilemap with the match-3 tiles
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3Int tilePos = new Vector3Int(tilemap.origin.x + x, tilemap.origin.y + y, 0);
                tilemap.SetTile(tilePos, tiles[Random.Range(0, 5)]);
            }
        }
    }
}
