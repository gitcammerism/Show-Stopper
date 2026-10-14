using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

// This script tracks the game state and handles the logic for the match-3 game.

public class Match3Game : MonoBehaviour
{
    public Grid grid;
    private Match3Board _board;
    
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
        ClearedTileCoordinates.Clear();
        DroppedTiles.Clear();
    }

    private void Awake()
    {
        _board = grid.GetComponent<Match3Board>();
    }
    
    // Starts a new game by creating & filling a new grid.
    public void StartNewGame()
    {
        TotalScore = 0;
        ClearedTileCoordinates = new();
        DroppedTiles = new();
        Scores = new();
    }
    
    // Attempts to move the selected tiles and returns if it was successful.
    public bool TryMove (Move move)
    {
        _scoreMultiplier = 1;

        _board.Swap(move.From, move.To);
        if (_board.FindMatches()) return true;

        _board.Swap(move.From, move.To);
        return false;
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
    //            if (_grid[x, y] == TileType.Obstacle) continue;

    //            // Searches for the next filled tile above the current hole
    //            int searchedY = -1;
    //            for (int checkY = y + 1; checkY < gridSize.y; checkY++)
    //            {
    //                if (_grid[x, checkY] == TileType.Obstacle) continue; // Ignores other obstacle tiles
    //                if (_grid[x, checkY] != TileType.None)
    //                {
    //                    searchedY = checkY;
    //                    break;
    //                }
    //            }

    //            // Once the next filled tile is found, drops it down to the current hole and adds it to DroppedTiles.
    //            if (searchedY != -1)
    //            {
    //                _grid[x, y] = _grid[x, searchedY];
    //                _grid[x, searchedY] = TileType.None;
    //                DroppedTiles.Add(new TileDrop(x, y, searchedY - y));
    //            }

    //            // OLD

    //            //// Checks for holes in the grid and adds them to DroppedTiles.
    //            //if (_grid[x, y] == TileType.None) holeCount += 1;
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
    //            if (_grid[x, y] == TileType.None)
    //            {
    //                _grid[x, y] = (TileType)Random.Range(1, 6);
    //                newTileCount += 1;
    //                int dropDistance = (gridSize.y + newTileCount - 1) - y; 
    //                DroppedTiles.Add(new TileDrop(x, y, dropDistance));
    //            }
    //        }
    //        // OLD
    //        //// Begins generating tiles to fill holes in the grid.
    //        //for (int h = 1; h <= holeCount; h++)
    //        //{
    //        //    _grid[x, gridSize.y - h] = (TileType)Random.Range(1, 6);
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
    //            if (_grid[c] != TileType.None)
    //            {
    //                _grid[c] = TileType.None;
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
