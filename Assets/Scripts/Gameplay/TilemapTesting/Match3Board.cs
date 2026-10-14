using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class Match3Board : MonoBehaviour
{
    [Header("Components")] 
    [SerializeField] private Tilemap boardTilemap;
    [SerializeField] private Match3Tile[] tiles;

    [Header("Board Settings")] 
    [SerializeField] private int width = 6;
    [SerializeField] private int height = 6;
    
    // 2D array grid holding the tiles
    private Match3Tile[,] tileGrid;
    
    void Start()
    {
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        tileGrid = new Match3Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Match3Tile tile;
                
                // Do-while to avoid instant matches spawned at game start
                do
                {
                    tile = tiles[Random.Range(0, tiles.Length)];
                }
                while (CreatesMatchAtPositions(x, y, tile));
                
                tileGrid[x, y] = tile;
                
                // Renders tile onto Unity Tilemap
                boardTilemap.SetTile(new Vector3Int(x, y, 0), tile.visualTile);
            }
        }
    }
    
    // Checks if there are any matches made at the given position with the given tile
    private bool CreatesMatchAtPositions(int x, int y, Match3Tile tile)
    {
        // Checks for horizontal matches to the left 
        if (x >= 2 && tileGrid[x - 1, y] == tile && tileGrid[x - 2, y] == tile) return true;

        // Checks for vertical matches above
        if (y >= 2 && tileGrid[x, y - 1] == tile && tileGrid[x, y - 2] == tile) return true;

        return false;
    }

    public void ProcessMatches()
    {
        HashSet<Vector2Int> tilesToClear = new();
        
        // Checks for horizontal matches 
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width - 2; x++)
            {
                Match3Tile current = tileGrid[x, y];
                if (current != null && tileGrid[x, y + 1] == current && tileGrid[x, y + 2] == current)
                {
                    tilesToClear.Add(new Vector2Int(x, y));
                    tilesToClear.Add(new Vector2Int(x, y + 1));
                    tilesToClear.Add(new Vector2Int(x, y + 2));
                }
            }
        }

        if (tilesToClear.Count > 0)
        {
            foreach (Vector2Int tile in tilesToClear)
            {
                tileGrid[tile.x, tile.y] = null;
                boardTilemap.SetTile(new Vector3Int(tile.x, tile.y, 0), null);
            }

            ApplyGravity();
        }
    }

    private void ApplyGravity()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tileGrid[x, y] == null)
                {
                    for (int nextY = y + 1; nextY < height; nextY++)
                    {
                        if (tileGrid[x, nextY] != null)
                        {
                            tileGrid[x, y] = tileGrid[x, nextY];
                            tileGrid[x, nextY] = null;
                            
                            boardTilemap.SetTile(new Vector3Int(x, y, 0), tileGrid[x, y].visualTile);
                            boardTilemap.SetTile(new Vector3Int(x, nextY, 0), null);
                            break;
                        }
                    }
                }
            }
        }

        RefillTopRow();
    }

    private void RefillTopRow()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tileGrid[x, y] == null)
                {
                    Match3Tile newTile = tiles[Random.Range(0, tiles.Length)];
                    tileGrid[x, y] = newTile;
                    boardTilemap.SetTile(new Vector3Int(x, y, 0), newTile.visualTile);
                }
            }
        }
        
        // Checks for new matches from filling
        ProcessMatches();
    }
}
