using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;
using static Unity.Mathematics.math;

/*
 * Last Modified: 09/23/2026 by Chandler Guzman
 * 
 * This script tracks the game state and handles the logic for the match-3 game.
 *
 * Chandler TO-DO:
 * - Continue working using tutorial (at 3.2 - Disappearing Tiles)
 *      -> https://catlikecoding.com/unity/tutorials/prototypes/match-3/#3.2
 */

public class Match3Game : MonoBehaviour
{
    // Default grid size is set to 8 x 8.
    [SerializeField] private int2 gridSize = 8;
    private MatchGrid2D<TileState> _grid;

    // Public getter properties for other classes so only Match3Game can modify grid state.
    public TileState this[int x, int y] => _grid[x, y];
    public TileState this[int2 c] => _grid[c];
    public int2 GridSize => gridSize;

    // List that stores the matches made by the player for processing.
    private List<Match> _matches;

    // List that stores tiles that have been matched.
    public List<int2> ClearedTileCoordinates 
    { get; private set; }

    // List that stores tiles that have dropped to fill in board gaps.
    public List<TileDrop> DroppedTiles
    { get; private set; }

    // Bool that tracks whether the game grid needs to be filled after a match.
    public bool NeedsFilling 
    { get; private set; }

    // Starts a new game by creating & filling a new grid.
    public void StartNewGame()
    {
        if (_grid.IsUndefined)
        {
            _grid = new(gridSize);
            _matches = new();
            ClearedTileCoordinates = new();
            DroppedTiles = new();
        }

        FillGrid();
    }

    // Fills up the game grid with random tiles.
    private void FillGrid()
    {
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                TileState a = TileState.None, b = TileState.None;
                int potentialMatchCount = 0;

                // Checks if there are any potential horizontal matches
                if (x > 1)
                {
                    a = _grid[x - 1, y];
                    if (a == _grid[x - 2, y])
                    {
                        potentialMatchCount = 1;
                    }
                }

                // Checks if there are any potential vertical  matches
                if (y > 1)
                {
                    b = _grid[x, y - 1];
                    if (b == _grid[x, y- 2])
                    {
                        potentialMatchCount += 1;

                        // If there's only a single match at this point, swap a and b.
                        // If there's two, a & b are ordered lowest to highest.
                        if (potentialMatchCount == 1) a = b;
                        else if (b < a) (a, b) = (b, a);
                    }
                }

                // Lowers the random range by however many tiles would cause a match to occur.
                TileState t = (TileState)Random.Range(1, 6 - potentialMatchCount);

                // Ensures a & b aren't picked if either one would result in a match.
                if (potentialMatchCount > 0 && t >= a) t += 1;
                if (potentialMatchCount == 2 && t >= b) t += 1;

                _grid[x, y] = t;
            }
        }
    }

    // Drops all tiles that need to fill gaps in the game board.
    public void DropTiles()
    {
        DroppedTiles.Clear();

        for (int x = 0; x < gridSize.x; x++)
        {
            int holeCount = 0;
            for (int y = 0; y < gridSize.y; y++)
            {
                // Checks for holes in the grid and adds them to DroppedTiles.
                if (_grid[x, y] == TileState.None) holeCount += 1;
                else if (holeCount > 0)
                {
                    _grid[x, y - holeCount] = _grid[x, y];
                    DroppedTiles.Add(new TileDrop(x, y - holeCount, holeCount));
                }
            }

            // Begins generating tiles to fill holes in the grid.
            for (int h = 1; h <= holeCount; h++)
            {
                _grid[x, gridSize.y - h] = (TileState)Random.Range(1, 6);
                DroppedTiles.Add(new TileDrop(x, gridSize.y - h, holeCount));
            }
        }

        NeedsFilling = false;

        // Look for any new matches as a result of tiles dropping.
        FindMatches();
    }

    // Returns if there are currently matches to process.
    public bool HasMatches => _matches.Count > 0;

    // Returns whether and matches were found.
    private bool FindMatches()
    {
        // Searches for horizontal matches.
        for (int y = 0; y < gridSize.y; y++)
        {
            // Start at the first horizontal tile on the grid.
            TileState start = _grid[0, y];
            int length = 1;

            // Loops through the rest of the row to search for matches.
            for (int x = 1; x < gridSize.x; x++)
            {
                TileState t = _grid[x, y];
                if (t == start) length += 1; // If there is a match, increase match length.
                else
                {
                    // If no more matches found but the length is >= 3 (match found in row) add to list then restart.
                    if (length >= 3) _matches.Add(new Match(x - length, y, length, true));
                    start = t;
                    length = 1;
                }
            }

            // Checks for a 3+  match at the end of the row then adds to the match list.
            if (length >= 3) _matches.Add(new Match(gridSize.x - length, y, length, true));
        }

        // Searches for vertical matches.
        for (int x = 0; x < gridSize.x; x++)
        {
            // Starts at the first vertical tile on the grid.
            TileState start = _grid[x, 0];
            int length = 1;

            // Loops through the rest of the column to search for matches.
            for (int y = 1; y < gridSize.y; y++)
            {
                TileState t = _grid[x, y];

                if (t == start) length += 1; // If there is a match, increase match length.
                else
                {
                    // If no more matches found but the length is >= 3 (match found in column) add to list then restart.
                    if (length >= 3) _matches.Add(new Match(x, y - length, length, false));
                    start = t;
                    length = 1;
                }
            }

            // Checks for a 3+  match at the end of the column then adds to the match list.
            if (length >= 3) _matches.Add(new Match(x, gridSize.y - length, length, false));
        }

        return HasMatches;
    }    

    // Attempts to move the selected tiles and returns if it was successful.
    public bool TryMove (Move move)
    {
        _grid.Swap(move.From, move.To);
        if (FindMatches()) return true;

        _grid.Swap(move.From, move.To);
        return false;
    }

    // Processes matches.
    public void ProcessMatches()
    {
        // Clears the cleared tile list.
        ClearedTileCoordinates.Clear();

        // Loops through all matches, clears their tiles, and adds their coordinates to the clear list.
        for (int m = 0; m < _matches.Count; m++)
        {
            Match match = _matches[m];
            int2 step = match.isHorizontal ? int2(1, 0) : int2(0, 1);
            int2 c = match.coordinates;

            for (int i = 0; i < match.length; c += step, i++)
            {
                if (_grid[c] != TileState.None)
                {
                    _grid[c] = TileState.None;
                    ClearedTileCoordinates.Add(c);
                }
            }
        }

        // Clears the match list and changes bool to true to indicate grid needs to be refilled.
        _matches.Clear();
        NeedsFilling = true;
    }
}
