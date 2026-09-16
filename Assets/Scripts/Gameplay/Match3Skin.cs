using TMPro;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

/*
 * Last Modified: 09/16/2026 by Chandler Guzman
 * 
 * This script tracks the current state of the game and starts a new game when prompted by Match3GameController.
 *
 * Chandler TO-DO:
 * - Continue working using tutorial (at 2.3 - Avoiding Immediate Matches)
 *      -> https://catlikecoding.com/unity/tutorials/prototypes/match-3/#1
 */

public class Match3Skin : MonoBehaviour
{
    [SerializeField] private Match3Game game;
    [SerializeField] private Tile[] tilePrefabs;

    private MatchGrid2D<Tile> _tiles;
    private float2 _tileOffset;

    public bool IsPlaying => true;
    public bool IsBusy => false;

    // Starts a new game by calling Match3Game.StartNewGame().
    public void StartNewGame () 
    {
        game.StartNewGame();

        // Ensures tiles will be centered on the origin.
        _tileOffset = -0.5f * (float2)(game.GridSize - 1);

        // Create new tiles grid if undefined, otherwise despawn all other tiles & set to null.
        if (_tiles.IsUndefined) _tiles = new(game.GridSize);
        else
        {
            for (int y = 0; y < _tiles.SizeY; y++)
            {
                for (int x = 0; x < _tiles.SizeX; x++)
                {
                    _tiles[x, y].Despawn();
                    _tiles[x, y] = null;
                }
            }
        }

        // Loops through tile grid and spawns tiles.
        for (int y = 0; y < _tiles.SizeY; y++)
        {
            for (int x = 0; x < _tiles.SizeX; x++)
            {
                _tiles[x, y] = SpawnTile(game[x, y], x, y);
            }
        }
    }

    // Spawns a random tile at the given location.
    private Tile SpawnTile (TileState t, float x, float y) =>
        tilePrefabs[(int)t - 1].Spawn(new Vector3(x + _tileOffset.x, y + _tileOffset.y));

    public void DoWork () {}

    // Handles dragging input using mouse click
    public bool EvaluateDrag (Vector3 start, Vector3 end)
    {
        return false;
    }
}
