using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;

public class Match3Board : MonoBehaviour
{
    [Header("Components")] 
    public Grid grid;
    public Tilemap boardTilemap;
    
    [Header("Prefabs")] 
    public Tile[] tilePrefabs;
    
    public Dictionary<Vector3, Tile> _boardTiles = new();
    private float anchor; 
    
    // Stores any possible moves in the game.
    public Move PossibleMove;

    // Score tracking variables.
    private int _scoreMultiplier;
    public int TotalScore { get; private set; }
    public List<SingleScore> Scores { get; private set; }
    
    // List that stores the matches made by the player for processing.
    private List<Match> _matches = new();
    
    // List that stores tiles that have been matched.
    public List<Vector3> ClearedTiles { get; private set; }
    
    // List that stores tiles that have dropped to fill in board gaps.
    public List<TileDrop> DroppedTiles { get; private set; }
    
    // Bool that tracks whether the game grid needs to be filled after a match.
    public bool NeedsFilling { get; private set; }
    
    // Event for when a match is made.
    public static event Action<Match> OnMatchMade;
    
    // Handles start logic.
    void Start()
    {
        anchor = boardTilemap.tileAnchor.x;
        
        // Refreshes all tiles, ensuring that the preview sprites won't be seen in-game.
        #if !UNITY_EDITOR
        var tilemaps = grid.GetComponentsInChildren<Tilemap>();
        foreach (var tilemap in tilemaps)
        {
            tilemap.RefreshAllTiles();
        }
        #endif
    }

    // Starts a new game and makes new lists.
    public void StartNewGame()
    {
        TotalScore = 0;
        ClearedTiles = new();
        DroppedTiles = new();
        Scores = new();
    }

    // Clears the game grid.
    public void ClearGrid()
    {
        _boardTiles.Clear();
        ClearedTiles.Clear();
        DroppedTiles.Clear();

        BoundsInt bounds = boardTilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            SpawnTile(pos);
        }
    }

    // Spawns a tile at the given position (called from TilePlacer).
    public void SpawnTile(Vector3Int position, Tile tile = null)
    {
        Vector3 worldPosition = grid.GetCellCenterWorld(position);
        Tile newTile = tile;
        
        // Adds to dictionary
        if (!_boardTiles.ContainsKey(worldPosition)) 
            _boardTiles.Add(worldPosition, tile);
        
        // Spawns new tile prefab as long as a match isn't made in doing so
        do
        {
            // Checks if there is already a tile there (previous failure) and destroy 
            if (_boardTiles[worldPosition] != null)
            {
                _boardTiles[worldPosition].Despawn();
            }

            // Creates new prefab tile
            newTile = tilePrefabs[UnityEngine.Random.Range(0, tilePrefabs.Length)].Spawn(worldPosition);

            // Stores prefab in dictionary
            _boardTiles[worldPosition] = newTile;
        } 
        while (CreatesMatch(worldPosition, newTile.tileType));
    }

    // Checks if, when placed at this position, the given tile creates a match
    private bool CreatesMatch(Vector3 position, TileType tileType)
    {
        // Checks for horizontal matches
        Vector3 left1 = new Vector3(position.x - 1, position.y, 0);
        Vector3 left2 = new Vector3(position.x - 2, position.y, 0);

        if (_boardTiles.ContainsKey(left1) && _boardTiles[left1].tileType == tileType &&
            _boardTiles.ContainsKey(left2) && _boardTiles[left2].tileType == tileType)
        {
            return true;
        }
        
        // Checks for vertical matches 
        Vector3 up1 = new Vector3(position.x, position.y - 1, 0);
        Vector3 up2 = new Vector3(position.x, position.y - 2, 0);
        
        if (_boardTiles.ContainsKey(up1) && _boardTiles[up1].tileType == tileType &&
            _boardTiles.ContainsKey(up2) && _boardTiles[up2].tileType == tileType)
        {
            return true;
        }

        return false;
    }

    // Converts the given screen position to a tile position.
    public Vector3 ScreenToTileSpace(Vector3 screenPosition)
    {
        Vector3Int newPos = grid.WorldToCell(screenPosition);
        Debug.Log("Position: " + grid.GetCellCenterWorld(newPos));
        return grid.GetCellCenterWorld(newPos);
    }

    public Vector3 WorldToLocalCell(Vector3 worldPosition)
    {
        Vector3Int newPos = boardTilemap.WorldToCell(worldPosition);
        Debug.Log("Tapped: " + boardTilemap.GetCellCenterLocal(newPos));
        return boardTilemap.GetCellCenterLocal(newPos);
    }

    // Checks if the given tile is valid.
    public bool IsValidTile(Vector3 position)
    {
        Vector3 worldPos = ScreenToTileSpace(position);
        if (_boardTiles.ContainsKey(worldPos))
        {
            return true;
        }
        return false;
    }

    // Attempts to move the selected tiles and returns if it was successful.
    public bool TryMove (Move move)
    {
        _scoreMultiplier = 1;

        Swap(move.From, move.To);
        if (FindMatches()) return true;

        Swap(move.From, move.To);
        return false;
    }
    
    // Swaps the given two tiles.
    public void Swap(Vector3 a, Vector3 b)
    {
        Vector3 worldA = ScreenToTileSpace(a);
        Vector3 worldB = ScreenToTileSpace(b);
        (_boardTiles[worldA], _boardTiles[worldB]) = (_boardTiles[worldB], _boardTiles[worldA]);
    }

    // Checks the game board to find matches.
    public bool FindMatches()
    {
        BoundsInt bounds = boardTilemap.cellBounds;
        
        // Searches for horizontal matches.
        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            // Start at the first horizontal tile on the grid.
            Vector3 startPos = grid.GetCellCenterWorld(new Vector3Int(0, y, 0));
            TileType start = _boardTiles[startPos].tileType;
            int length = 1;

            // Loops through the rest of the row to search for matches.
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                Vector3 currentPos = grid.GetCellCenterWorld(new Vector3Int(x, y, 0));
                TileType t = _boardTiles[currentPos].tileType;
                if (t == start && start != TileType.None)
                {                 
                    Debug.Log(start + " and " + t + " match");
                    Debug.Log("(" + startPos + ") & (" + currentPos + ")");
                    length += 1; // If there is a match, increase match length.
                }
                else
                {
                    // If no more matches found but the length is >= 3 (match found in row) add to list then restart.
                    if (length >= 3 && start != TileType.None)
                    {
                        _matches.Add(new Match(
                            grid.GetCellCenterWorld(new Vector3Int(x - length, y, 0)),
                            length, true, start)
                        );
                    }

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
            Vector3 startPos = grid.GetCellCenterWorld(new Vector3Int(x, 0, 0));
            TileType start = _boardTiles[startPos].tileType;
            int length = 1;

            // Loops through the rest of the column to search for matches.
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3 currentPos = grid.GetCellCenterWorld(new Vector3Int(x, y, 0));
                TileType t = _boardTiles[currentPos].tileType;
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

        // Clamps values so that it can't go outside of the grid bounds
        for (int i = 0; i < _matches.Count; i++)
        {
            Match temp = _matches[i];
            temp.location.x = Mathf.Clamp(temp.location.x, bounds.xMin + anchor, bounds.xMax - anchor);
            temp.location.y = Mathf.Clamp(temp.location.y, bounds.yMin + anchor, bounds.yMax - anchor);
            _matches[i] = temp;
        }

        return _matches.Count > 0;
    }
    
    // Quickly returns if there are any matches in the board.
    public bool HasMatches() => _matches.Count > 0;
    
    // Processes matches.
    public void ProcessMatches()
    {
        // Clears necessary lists.
        ClearedTiles.Clear();
        Scores.Clear();

        // Loops through all matches, clears their tiles, and adds their coordinates to the clear list.
        for (int m = 0; m < _matches.Count; m++)
        {
            Match match = _matches[m];
            Vector3 step = match.isHorizontal ? new Vector3(1, 0, 0) : new Vector3(0, 1, 0);
            Vector3 c = match.location;
            Debug.Log(c);
            
            for (int i = 0; i < match.length; c += step, i++)
            {
                if (_boardTiles[c].tileType != TileType.None)
                {
                    _boardTiles[c].tileType = TileType.None;
                    ClearedTiles.Add(c);
                }
            }

            // The more matches you get from a single move, the higher your score gets.
            var score = new SingleScore
            {
                position = (match.location + step) * (match.length - 1) * 0.5f,
                value = match.length * _scoreMultiplier++
            };
            Scores.Add(score);
            TotalScore += score.value;

            OnMatchMade?.Invoke(match);
        }

        // Clears the match list and changes bool to true to indicate grid needs to be refilled.
        _matches.Clear();
        NeedsFilling = true;
    }
    
    // Drops all tiles that need to fill gaps in the game board.
    public void DropTiles()
    {
        Debug.Log("Board DropTiles()");
        DroppedTiles.Clear();
        BoundsInt bounds = boardTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {              
                Vector3 currentPos = grid.GetCellCenterWorld(new Vector3Int(x, y, 0));
                // Checks if tile is an obstacle, and ignores it.
                // if (_boardTiles[currentPos].tileType == TileType.Obstacle) continue;

                // Searches for the next filled tile above the current hole
                int searchedY = -1;
                for (int checkY = y + 1; checkY < bounds.yMax; checkY++)
                {
                    Vector3 checkPos = grid.GetCellCenterWorld(new Vector3Int(x, checkY, 0));
                    // if (_boardTiles[checkPos].tileType == TileType.Obstacle) continue; // Ignores other obstacle tiles
                    if (_boardTiles[checkPos].tileType != TileType.None)
                    {
                        searchedY = checkY;
                        break;
                    }
                }
                
                // Once the next filled tile is found, drops it down to the current hole and adds it to DroppedTiles.
                if (searchedY != -1)
                {
                    Vector3 searchPos = grid.GetCellCenterWorld(new Vector3Int(x, searchedY, 0));
                    _boardTiles[currentPos] = _boardTiles[searchPos];
                    _boardTiles[searchPos].tileType = TileType.None;
                    DroppedTiles.Add(new TileDrop(currentPos, searchedY - y));
                }
            }
            
            // Generates new random tiles to fill in the holes in the grid. 
            int newTileCount = 0;
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3 tilePos = grid.GetCellCenterWorld(new Vector3Int(x, y, 0));
                if (_boardTiles[tilePos].tileType == TileType.None)
                {
                    //Tile newTile = tilePrefabs[UnityEngine.Random.Range(0, tilePrefabs.Length)].Spawn(tilePos);
                    //_boardTiles[tilePos].tileType = (TileType)UnityEngine.Random.Range(1, 6);
                    newTileCount += 1;
                    int dropDistance = (bounds.yMax + newTileCount - 1) - y; 
                    DroppedTiles.Add(new TileDrop(tilePos, dropDistance));
                }
            }
        }

        NeedsFilling = false;

        // Look for any new matches as a result of tiles dropping.
        if (!FindMatches())
        {
            //PossibleMove = Move.FindMove(this);
        }
    }
}
