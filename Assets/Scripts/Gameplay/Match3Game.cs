using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;
using static Unity.Mathematics.math;

/*
 * Last Modified: 09/16/2026 by Chandler Guzman
 * 
 * This script tracks the game state and handles the logic for the match-3 game.
 *
 * Chandler TO-DO:
 * - 
 */

public class Match3Game : MonoBehaviour
{
    // Default grid size is set to 8 x 8.
    [SerializeField] private int2 gridSize = 8;
    private MatchGrid2D<TileState> _grid;

    // Public getter properties for other classes so only Match3Game can modify grid state
    public TileState this[int x, int y] => _grid[x, y];
    public TileState this[int2 c] => _grid[c];
    public int2 GridSize => gridSize;

    // Starts a new game by creating & filling a new grid.
    public void StartNewGame ()
    {
        if (_grid.IsUndefined) _grid = new(gridSize);
        FillGrid();
    }

    // Fills up the game grid with random tiles.
    private void FillGrid ()
    {
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                _grid[x, y] = (TileState)Random.Range(1, 6);
            }
        }
    }
}
