using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;
using static Unity.Mathematics.math;

// This script tracks the game state and handles the logic for the match-3 game.

public class Match3Game : MonoBehaviour
{
    // Default grid size is set to 8 x 8.
    [SerializeField] private int2 gridSize = 8;
    private MatchGrid2D<TileState> _grid;
    
    public GridShape gridShape = GridShape.Square;

    // Public getter properties for other classes so only Match3Game can modify grid state.
    public TileState this[int x, int y] => _grid[x, y];
    public TileState this[int2 c] => _grid[c];
    public int2 GridSize => gridSize;

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
        _grid = new(gridSize);
        _matches.Clear();
        ClearedTileCoordinates.Clear();
        DroppedTiles.Clear();
    }
    
    // Starts a new game by creating & filling a new grid.
    public void StartNewGame()
    {
        TotalScore = 0;

        if (_grid.IsUndefined)
        {
            _grid = new(gridSize);
            _matches = new();
            ClearedTileCoordinates = new();
            DroppedTiles = new();
            Scores = new();
        }

        // Continues filling the grid until it begins with at least 1 valid move.
        switch (gridShape)
        {
            case GridShape.Circle:
                do
                {
                    FillCircleGrid();
                    PossibleMove = Move.FindMove(this);
                }
                while (!PossibleMove.IsValid);
                break;
            
            case GridShape.Heart:
                do
                {
                    FillHeartGrid();
                    PossibleMove = Move.FindMove(this);
                }
                while (!PossibleMove.IsValid);
                break;
                
            case GridShape.Square:
            default:
                do
                {
                    FillGrid();
                    PossibleMove = Move.FindMove(this);
                }
                while (!PossibleMove.IsValid);
                break;
        }
    }

    // Fills up the game grid with random tiles.
    public void FillGrid()
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
    
    public void FillCircleGrid()
    {
        // Calculate circle center and radius based on grid size
        float centerX = (gridSize.x - 1) * 0.5f;
        float centerY = (gridSize.y - 1) * 0.5f;
        float gridRadius = Mathf.Min(gridSize.x, gridSize.y) * 0.5f;

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                // 1. Check if this coordinate falls outside the circle boundary
                float distanceFromCenter = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                if (distanceFromCenter > gridRadius)
                {
                    _grid[x, y] = TileState.None; // Mark out-of-bounds slots as None
                    continue; // Skip the rest of the loop for this slot
                }

                TileState a = TileState.None, b = TileState.None;
                int potentialMatchCount = 0;

                // 2. Checks if there are any potential horizontal matches (and ensure neighbor isn't None)
                if (x > 1)
                {
                    a = _grid[x - 1, y];
                    if (a != TileState.None && a == _grid[x - 2, y])
                    {
                        potentialMatchCount = 1;
                    }
                }

                // 3. Checks if there are any potential vertical matches (and ensure neighbor isn't None)
                if (y > 1)
                {
                    b = _grid[x, y - 1];
                    if (b != TileState.None && b == _grid[x, y - 2])
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

    public void FillHeartGrid()
    {
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                if (!IsInsideHeart(x, y))
                {
                    _grid[x, y] = TileState.None;
                    continue;
                }

                TileState a = TileState.None, b = TileState.None;
                int potentialMatchCount = 0;

                if (x > 1)
                {
                    a = _grid[x - 1, y];
                    if (a != TileState.None && a == _grid[x - 2, y])
                    {
                        potentialMatchCount = 1;
                    }
                }

                if (y > 1)
                {
                    b = _grid[x, y - 1];
                    if (b != TileState.None && b == _grid[x, y - 2])
                    {
                        potentialMatchCount += 1;
                        if (potentialMatchCount == 1) a = b;
                        else if (b < a) (a, b) = (b, a);
                    }
                }

                TileState t = (TileState)Random.Range(1, 6 - potentialMatchCount);
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
        if (!FindMatches())
        {
            PossibleMove = Move.FindMove(this);
        }
    }
    
    // Drops all tiles that need to fill gaps in the game board.
    public void DropTilesCircle()
    {
        DroppedTiles.Clear();

        float centerX = (gridSize.x - 1) * 0.5f;
        float centerY = (gridSize.y - 1) * 0.5f;
        float gridRadius = Mathf.Min(gridSize.x, gridSize.y) * 0.5f;

        for (int x = 0; x < gridSize.x; x++)
        {
            // 1. Get all valid y-indices inside the circle for this column, sorted bottom-to-top
            List<int> validYIndices = new List<int>();
            for (int y = 0; y < gridSize.y; y++)
            {
                if (Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY)) <= gridRadius)
                {
                    validYIndices.Add(y);
                }
            }

            if (validYIndices.Count == 0) continue;

            // 2. Make tiles fall down to fill holes within valid circle slots
            for (int i = 0; i < validYIndices.Count; i++)
            {
                int currentY = validYIndices[i];

                if (_grid[x, currentY] == TileState.None)
                {
                    int foundTileY = -1;
                    for (int j = i + 1; j < validYIndices.Count; j++)
                    {
                        int checkY = validYIndices[j];
                        if (_grid[x, checkY] != TileState.None)
                        {
                            foundTileY = checkY;
                            break;
                        }
                    }

                    if (foundTileY != -1)
                    {
                        _grid[x, currentY] = _grid[x, foundTileY];
                        _grid[x, foundTileY] = TileState.None;
                        
                        int dropDistance = foundTileY - currentY;
                        // Using your struct constructor: (x, y, distance) where y is the destination row
                        DroppedTiles.Add(new TileDrop(x, currentY, dropDistance));
                    }
                }
            }

            // 3. Spawn new random tiles in the remaining empty slots at the top of the column
            for (int i = 0; i < validYIndices.Count; i++)
            {
                int currentY = validYIndices[i];
                if (_grid[x, currentY] == TileState.None)
                {
                    TileState t = (TileState)Random.Range(1, 6);
                    _grid[x, currentY] = t;
                    
                    int topY = validYIndices[validYIndices.Count - 1];
                    int spawnDistance = (topY + 1) - currentY;
                    
                    // Using your struct constructor for spawned tiles coming from above
                    DroppedTiles.Add(new TileDrop(x, currentY, spawnDistance));
                }
            }
        }

        NeedsFilling = false;

        if (!FindMatches())
        {
            PossibleMove = Move.FindMove(this);
        }
    }

    public void DropTilesHeart()
    {
        DroppedTiles.Clear();

        for (int x = 0; x < gridSize.x; x++)
        {
            List<int> validYIndices = new List<int>();
            for (int y = 0; y < gridSize.y; y++)
            {
                if (IsInsideHeart(x, y))
                {
                    validYIndices.Add(y);
                }
            }

            if (validYIndices.Count == 0) continue;

            // Slide existing tiles down within valid heart slots
            for (int i = 0; i < validYIndices.Count; i++)
            {
                int currentY = validYIndices[i];

                if (_grid[x, currentY] == TileState.None)
                {
                    int foundTileY = -1;
                    for (int j = i + 1; j < validYIndices.Count; j++)
                    {
                        int checkY = validYIndices[j];
                        if (_grid[x, checkY] != TileState.None)
                        {
                            foundTileY = checkY;
                            break;
                        }
                    }

                    if (foundTileY != -1)
                    {
                        _grid[x, currentY] = _grid[x, foundTileY];
                        _grid[x, foundTileY] = TileState.None;
                        
                        int dropDistance = foundTileY - currentY;
                        DroppedTiles.Add(new TileDrop(x, currentY, dropDistance));
                    }
                }
            }

            // Spawn new tiles at the top of the heart columns
            for (int i = 0; i < validYIndices.Count; i++)
            {
                int currentY = validYIndices[i];
                if (_grid[x, currentY] == TileState.None)
                {
                    TileState t = (TileState)Random.Range(1, 6);
                    _grid[x, currentY] = t;
                    
                    int topY = validYIndices[validYIndices.Count - 1];
                    int spawnDistance = (topY + 1) - currentY;
                    DroppedTiles.Add(new TileDrop(x, currentY, spawnDistance));
                }
            }
        }

        NeedsFilling = false;

        if (!FindMatches())
        {
            PossibleMove = Move.FindMove(this);
        }
    }

    public bool IsInsideHeart(int x, int y)
    {
        // Ensure coordinates stay within the 6x6 bounds
        if (x < 0 || x > 5 || y < 0 || y > 5) return false;

        // Define the valid X columns for each Y row (from bottom y=0 to top y=5)
        switch (y)
        {
            case 5: // Top row (lobes)
                return x >= 1 && x <= 4;
            case 4: // Upper-middle (wide body)
            case 3: 
                return x >= 0 && x <= 5;
            case 2: // Lower-middle (tapering in)
                return x >= 1 && x <= 4;
            case 1: // Lower body
                return x >= 2 && x <= 3;
            case 0: // Bottom point
                return x == 2 || x == 3;
            default:
                return false;
        }
    }

    // Returns if there are currently matches to process.
    public bool HasMatches => _matches.Count > 0;

    // Returns whether and matches were found.
    public bool FindMatches()
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
                if (start == TileState.None || t == TileState.None) continue;
                if (t == start) length += 1; // If there is a match, increase match length.
                else
                {
                    // If no more matches found but the length is >= 3 (match found in row) add to list then restart.
                    if (length >= 3) _matches.Add(new Match(x - length, y, length, true, start));
                    start = t;
                    length = 1;
                }
            }

            // Checks for a 3+  match at the end of the row then adds to the match list.
            if (length >= 3) _matches.Add(new Match(gridSize.x - length, y, length, true, start));
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
                if (start == TileState.None || t == TileState.None) continue;
                if (t == start) length += 1; // If there is a match, increase match length.
                else
                {
                    // If no more matches found but the length is >= 3 (match found in column) add to list then restart.
                    if (length >= 3) _matches.Add(new Match(x, y - length, length, false, start));
                    start = t;
                    length = 1;
                }
            }

            // Checks for a 3+  match at the end of the column then adds to the match list.
            if (length >= 3) _matches.Add(new Match(x, gridSize.y - length, length, false, start));
        }

        return HasMatches;
    }    

    // Attempts to move the selected tiles and returns if it was successful.
    public bool TryMove (Move move)
    {
        _scoreMultiplier = 1;

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
        Scores.Clear();

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

            // The more matches you get from a single move, the higher your score gets.
            var score = new SingleScore
            {
                position = match.coordinates + (float2)step * (match.length - 1) * 0.5f,
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
}
