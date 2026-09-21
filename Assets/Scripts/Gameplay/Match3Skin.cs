using TMPro;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

/*
 * Last Modified: 09/21/2026 by Chandler Guzman
 * 
 * This script tracks & handles the current state of the game and starts a new game when prompted by Match3GameController.
 *
 * Chandler TO-DO:
 * - Continue working using tutorial (at 2.7 - Filling Holes)
 *      -> https://catlikecoding.com/unity/tutorials/prototypes/match-3/#2.7
 */

public class Match3Skin : MonoBehaviour
{
    [SerializeField] private Match3Game game;
    [SerializeField] private Tile[] tilePrefabs;
    
    [SerializeField, Range(0.1f, 1f)]
    private float _dragThreshold = 0.5f;

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

    // If there are matches, process them.
    public void DoWork () 
    {
        if (game.HasMatches) ProcessMatches();
    }

    // Invokes Match3Game's ProcessMatches and despawns all cleared tiles.
    private void ProcessMatches ()
    {
        game.ProcessMatches();

        for (int i = 0; i < game.ClearedTileCoordinates.Count; i++)
        {
            int2 c = game.ClearedTileCoordinates[i];
            _tiles[c].Despawn();
            _tiles[c] = null;
        }
    }

    // Handles dragging input using mouse click.
    public bool EvaluateDrag (Vector3 start, Vector3 end)
    {
        // Determines the tile position of a & b then creates a Move struct based on the move performed.
        float2 a = ScreenToTileSpace(start), b = ScreenToTileSpace(end);
        var move = new Move(
            (int2)floor(a), (b - a) switch
            {
                var d when d.x > _dragThreshold => MoveDirection.Right,
                var d when d.x < -_dragThreshold => MoveDirection.Left,
                var d when d.y > _dragThreshold => MoveDirection.Up,
                var d when d.y < -_dragThreshold => MoveDirection.Down,
                _ => MoveDirection.None
            }
        );

        // If the move is valid and the new coordinates are valid, the tiles can be moved.
        if (move.IsValid && _tiles.AreValidCoordinates(move.From) && _tiles.AreValidCoordinates(move.To))
        {
            DoMove(move);
            return false;
        }
        return true;
    }

    // Attempts the given move and swaps both tiles if successful.
    private void DoMove (Move move)
    {
        if (game.TryMove(move))
        {
            (_tiles[move.From].transform.localPosition, _tiles[move.To].transform.localPosition)
                =
            (_tiles[move.To].transform.localPosition, _tiles[move.From].transform.localPosition);
            _tiles.Swap(move.From, move.To);
        }
    }

    // Converts the given screen space coordinations to the actual tile coordinates.
    private float2 ScreenToTileSpace (Vector3 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Vector3 p = ray.origin - ray.direction * (ray.origin.z / ray.direction.z);
        return float2(p.x - _tileOffset.x + 0.5f, p.y - _tileOffset.y + 0.5f);
    }
}
