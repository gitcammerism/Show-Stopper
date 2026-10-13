using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class Match3Board : MonoBehaviour
{
    [Header("Components")] 
    public Grid grid;
    [SerializeField] private Tilemap boardTilemap;

    [Header("Prefabs")] 
    public Tile[] tilePrefabs;
    
    public Dictionary<Vector3Int, Tile> _boardTiles = new();
    
    // List that stores the matches made by the player for processing.
    private List<Match> _matches = new();
    
    void Start()
    {
        // Refreshes all tiles, ensuring that the preview sprites won't be seen in-game.
        #if !UNITY_EDITOR
        var tilemaps = grid.GetComponentsInChildren<Tilemap>();
        foreach (var tilemap in tilemaps)
        {
            tilemap.RefreshAllTiles();
        }
        #endif
    }

    // Spawns a tile at the given position (called from TilePlacer).
    public void SpawnTile(Vector3Int position, Tile tile = null)
    {
        // Adds to dictionary
        if (!_boardTiles.ContainsKey(position)) 
            _boardTiles.Add(position, tile);

        Tile newTile = tile;
        Vector3 worldPosition = grid.GetCellCenterWorld(position);
        
        // Spawns new tile prefab as long as a match isn't made in doing so
        do
        {
            // Checks if there is already a tile there (previous failure) and destroy 
            if (_boardTiles[position] != null)
            {
                Destroy(_boardTiles[position].gameObject);
            }
            
            // Creates new prefab tile
            newTile = Instantiate(
                tilePrefabs[Random.Range(0, tilePrefabs.Length)],
                worldPosition,
                Quaternion.identity
            );

            // Stores prefab in dictionary
            _boardTiles[position] = newTile;
        } 
        while (CreatesMatch(position, newTile.tileType));
    }

    // Checks if, when placed at this position, the given tile creates a match
    private bool CreatesMatch(Vector3Int position, TileType tileType)
    {
        // Checks for horizontal matches
        Vector3Int left1 = new Vector3Int(position.x - 1, position.y, 0);
        Vector3Int left2 = new Vector3Int(position.x - 2, position.y, 0);

        if (_boardTiles.ContainsKey(left1) && _boardTiles[left1].tileType == tileType &&
            _boardTiles.ContainsKey(left2) && _boardTiles[left2].tileType == tileType)
        {
            return true;
        }
        
        // Checks for vertical matches 
        Vector3Int up1 = new Vector3Int(position.x, position.y - 1, 0);
        Vector3Int up2 = new Vector3Int(position.x, position.y - 2, 0);
        
        if (_boardTiles.ContainsKey(up1) && _boardTiles[up1].tileType == tileType &&
            _boardTiles.ContainsKey(up2) && _boardTiles[up2].tileType == tileType)
        {
            return true;
        }

        return false;
    }

    public Vector3 ScreenToTileSpace(Vector3 screenPosition)
    {
        Vector3Int newPos = grid.WorldToCell(screenPosition);
        return grid.GetCellCenterWorld(newPos);
    }

    public bool IsValidTile(Vector3 position)
    {
        if (_boardTiles.ContainsKey(grid.WorldToCell(position)))
            return true;
        return false;
    }

    public void Swap(Vector3 a, Vector3 b)
    {
        var temp = _boardTiles[grid.WorldToCell(a)];
        _boardTiles[grid.WorldToCell(a)] = _boardTiles[grid.WorldToCell(b)];
        _boardTiles[grid.WorldToCell(b)] = temp;
    }

    public bool FindMatches()
    {
        BoundsInt bounds = boardTilemap.cellBounds;
        
        // Searches for horizontal matches.
        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            // Start at the first horizontal tile on the grid.
            TileType start = _boardTiles[new Vector3Int(0, y, 0)].tileType;
            int length = 1;

            // Loops through the rest of the row to search for matches.
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                TileType t = _boardTiles[new Vector3Int(x, y, 0)].tileType;
                if (t == start && start != TileType.None) length += 1; // If there is a match, increase match length.
                else
                {
                    // If no more matches found but the length is >= 3 (match found in row) add to list then restart.
                    if (length >= 3 && start != TileType.None)
                        _matches.Add(new Match(
                            grid.GetCellCenterWorld(new Vector3Int(x - length, y, 0)), 
                            length, true, start)
                        );
                    
                    start = t;
                    length = 1;
                }
            }

            // Checks for a 3+  match at the end of the row then adds to the match list.
            if (length >= 3 && start != TileType.None) 
                _matches.Add(new Match(
                    grid.GetCellCenterWorld(new Vector3Int(bounds.xMax - length, y, 0)), 
                    length, true, start)
                );
        }

        // Searches for vertical matches.
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            // Starts at the first vertical tile on the grid.
            TileType start = _boardTiles[new Vector3Int(x, 0, 0)].tileType;
            int length = 1;

            // Loops through the rest of the column to search for matches.
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                TileType t = _boardTiles[new Vector3Int(x, y, 0)].tileType;
                if (t == start && start != TileType.None) length += 1; // If there is a match, increase match length.
                else
                {
                    // If no more matches found but the length is >= 3 (match found in column) add to list then restart.
                    if (length >= 3 && start != TileType.None) 
                        _matches.Add(new Match(
                            grid.GetCellCenterWorld(new Vector3Int(x, y - length, 0)),
                            length, false, start)
                        );
                    start = t;
                    length = 1;
                }
            }

            // Checks for a 3+  match at the end of the column then adds to the match list.
            if (length >= 3 && start != TileType.None) 
                _matches.Add(new Match(
                        grid.GetCellCenterWorld(new Vector3Int(x, bounds.yMax - length, 0)),
                        length, false, start)
                );
        }

        return _matches.Count > 0;
    }
}
