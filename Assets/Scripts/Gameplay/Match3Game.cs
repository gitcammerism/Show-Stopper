using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Unity.Mathematics.math;
using Random = UnityEngine.Random;

// This script tracks the game state and handles the logic for the match-3 game.

public class Match3Game : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase[] tiles;
    private Vector3Int gridSize;

    // List that stores the matches made by the player for processing.
    private List<Match> _matches;

    // Stores any possible moves in the game.
    public Move PossibleMove;

    // Score tracking variables.
    public int TotalScore
    { get; private set; }

    public List<SingleScore> Scores 
    { get; private set; }

    private int _scoreMultiplier;

    // List that stores tiles that have been matched.
    public List<int2> ClearedTileCoordinates 
    { get; private set; }

    // List that stores tiles that have dropped to fill in board gaps.
    public List<TileDrop> DroppedTiles
    { get; private set; }

    // Bool that tracks whether the game grid needs to be filled after a match.
    public bool NeedsFilling 
    { get; private set; }

    public static event Action<Match> OnMatchMade;

    public void ClearGrid()
    {
        _matches.Clear();
        ClearedTileCoordinates.Clear();
        DroppedTiles.Clear();
    }
    
    // Starts a new game by creating & filling a new grid.
    public void StartNewGame()
    {
        TotalScore = 0;
        _matches = new();
        ClearedTileCoordinates = new();
        DroppedTiles = new();
        Scores = new();

        tilemap.CompressBounds();
        gridSize = tilemap.size;
        FillGrid();
    }
    
    // Fills up the game grid with random tiles.
    public void FillGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3Int tilePos = new Vector3Int(tilemap.origin.x + x, tilemap.origin.y + y, 0);
                TileBase tile = tilemap.GetTile(tilePos);
                if (tile)
                tilemap.SetTile(tilePos, tiles[Random.Range(0, tiles.Length)]);
            }
        }
    }

    // Drops all tiles that need to fill gaps in the game board.
    //public void DropTiles()
    //{
    //    DroppedTiles.Clear();

    //    for (int x = 0; x < gridSize.x; x++)
    //    {
    //        // OLD
    //        //int holeCount = 0;

    //        for (int y = 0; y < gridSize.y; y++)
    //        {
    //            // Checks if tile is an obstacle, and ignores it.
    //            if (_grid[x, y] == TileState.Obstacle) continue;

    //            // Searches for the next filled tile above the current hole
    //            int searchedY = -1;
    //            for (int checkY = y + 1; checkY < gridSize.y; checkY++)
    //            {
    //                if (_grid[x, checkY] == TileState.Obstacle) continue; // Ignores other obstacle tiles
    //                if (_grid[x, checkY] != TileState.None)
    //                {
    //                    searchedY = checkY;
    //                    break;
    //                }
    //            }

    //            // Once the next filled tile is found, drops it down to the current hole and adds it to DroppedTiles.
    //            if (searchedY != -1)
    //            {
    //                _grid[x, y] = _grid[x, searchedY];
    //                _grid[x, searchedY] = TileState.None;
    //                DroppedTiles.Add(new TileDrop(x, y, searchedY - y));
    //            }

    //            // OLD

    //            //// Checks for holes in the grid and adds them to DroppedTiles.
    //            //if (_grid[x, y] == TileState.None) holeCount += 1;
    //            //else if (holeCount > 0)
    //            //{
    //            //    _grid[x, y - holeCount] = _grid[x, y];
    //            //    DroppedTiles.Add(new TileDrop(x, y - holeCount, holeCount));
    //            //}
    //        }

    //        // Generates new random tiles to fill in the holes in the grid. 
    //        int newTileCount = 0;
    //        for (int y = 0; y < gridSize.y; y++)
    //        {
    //            if (_grid[x, y] == TileState.None)
    //            {
    //                _grid[x, y] = (TileState)Random.Range(1, 6);
    //                newTileCount += 1;
    //                int dropDistance = (gridSize.y + newTileCount - 1) - y; 
    //                DroppedTiles.Add(new TileDrop(x, y, dropDistance));
    //            }
    //        }
    //        // OLD
    //        //// Begins generating tiles to fill holes in the grid.
    //        //for (int h = 1; h <= holeCount; h++)
    //        //{
    //        //    _grid[x, gridSize.y - h] = (TileState)Random.Range(1, 6);
    //        //    DroppedTiles.Add(new TileDrop(x, gridSize.y - h, holeCount));
    //        //}
    //    }

    //    NeedsFilling = false;

    //    // Look for any new matches as a result of tiles dropping.
    //    if (!FindMatches())
    //    {
    //        PossibleMove = Move.FindMove(this);
    //    }
    //}

    // Returns if there are currently matches to process.
    public bool HasMatches => _matches.Count > 0;

    // Returns whether and matches were found.
    //public bool FindMatches()
    //{
    //    // Searches for horizontal matches.
    //    for (int y = 0; y < gridSize.y; y++)
    //    {
    //        // Start at the first horizontal tile on the grid.
    //        TileState start = _grid[0, y];
    //        int length = 1;

    //        // Loops through the rest of the row to search for matches.
    //        for (int x = 1; x < gridSize.x; x++)
    //        {
    //            TileState t = _grid[x, y];
    //            if (t == start && start != TileState.None && start != TileState.Obstacle) length += 1; // If there is a match, increase match length.
    //            else
    //            {
    //                // If no more matches found but the length is >= 3 (match found in row) add to list then restart.
    //                if (length >= 3 && start != TileState.None && start != TileState.Obstacle) _matches.Add(new Match(x - length, y, length, true, start));
    //                start = t;
    //                length = 1;
    //            }
    //        }

    //        // Checks for a 3+  match at the end of the row then adds to the match list.
    //        if (length >= 3 && start != TileState.None && start != TileState.Obstacle) _matches.Add(new Match(gridSize.x - length, y, length, true, start));
    //    }

    //    // Searches for vertical matches.
    //    for (int x = 0; x < gridSize.x; x++)
    //    {
    //        // Starts at the first vertical tile on the grid.
    //        TileState start = _grid[x, 0];
    //        int length = 1;

    //        // Loops through the rest of the column to search for matches.
    //        for (int y = 1; y < gridSize.y; y++)
    //        {
    //            TileState t = _grid[x, y];
    //            if (t == start && start != TileState.None && start != TileState.Obstacle) length += 1; // If there is a match, increase match length.
    //            else
    //            {
    //                // If no more matches found but the length is >= 3 (match found in column) add to list then restart.
    //                if (length >= 3 && start != TileState.None && start != TileState.Obstacle) _matches.Add(new Match(x, y - length, length, false, start));
    //                start = t;
    //                length = 1;
    //            }
    //        }

    //        // Checks for a 3+  match at the end of the column then adds to the match list.
    //        if (length >= 3 && start != TileState.None && start != TileState.Obstacle) _matches.Add(new Match(x, gridSize.y - length, length, false, start));
    //    }

    //    return HasMatches;
    //}    

    // Attempts to move the selected tiles and returns if it was successful.
    //public bool TryMove (Move move)
    //{
    //    // Returns false if either tile is an obstacle tile.
    //    if (_grid[move.From] == TileState.Obstacle || _grid[move.To] == TileState.Obstacle) return false;
        
    //    _scoreMultiplier = 1;

    //    _grid.Swap(move.From, move.To);
    //    if (FindMatches()) return true;

    //    _grid.Swap(move.From, move.To);
    //    return false;
    //}

    //// Processes matches.
    //public void ProcessMatches()
    //{
    //    // Clears the cleared tile list.
    //    ClearedTileCoordinates.Clear();
    //    Scores.Clear();

    //    // Loops through all matches, clears their tiles, and adds their coordinates to the clear list.
    //    for (int m = 0; m < _matches.Count; m++)
    //    {
    //        Match match = _matches[m];
    //        int2 step = match.isHorizontal ? int2(1, 0) : int2(0, 1);
    //        int2 c = match.coordinates;

    //        for (int i = 0; i < match.length; c += step, i++)
    //        {
    //            if (_grid[c] != TileState.None)
    //            {
    //                _grid[c] = TileState.None;
    //                ClearedTileCoordinates.Add(c);
    //            }
    //        }

    //        // The more matches you get from a single move, the higher your score gets.
    //        var score = new SingleScore
    //        {
    //            position = match.coordinates + (float2)step * (match.length - 1) * 0.5f,
    //            value = match.length * _scoreMultiplier++
    //        };
    //        Scores.Add(score);
    //        TotalScore += score.value;

    //        OnMatchMade?.Invoke(match);
    //    }

    //    // Clears the match list and changes bool to true to indicate grid needs to be refilled.
    //    _matches.Clear();
    //    NeedsFilling = true;
    //}
}
