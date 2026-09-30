using TMPro;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

// This script tracks & handles the current state of the game and starts a new game when prompted by Match3GameController.

public class Match3Skin : MonoBehaviour
{
    // Game setup.
    [SerializeField] private TextMeshPro gameOverText, totalScoreText;
    [SerializeField] private Match3Game game;
    [SerializeField] private Tile[] tilePrefabs;
    [SerializeField] private FloatingScore floatingScorePrefab;
    private float _floatingScoreZ;
    private MatchGrid2D<Tile> _tiles;
    private float2 _tileOffset;

    // Drag threshold for input.
    [SerializeField, Range(0.1f, 1f)]
    private float _dragThreshold = 0.5f;

    // Tile animation setup.
    [SerializeField] private TileSwapper tileSwapper;
    private float _busyDuration;

    [SerializeField, Range(0.1f, 20f)]
    private float dropSpeed = 8f;

    [SerializeField, Range(0f, 10f)]
    private float newDropOffset = 2f;
       
    public bool IsPlaying => IsBusy || game.PossibleMove.IsValid;
    public bool IsBusy => _busyDuration > 0f;

    // Starts a new game by calling Match3Game.StartNewGame().
    public void StartNewGame () 
    {
        _busyDuration = 0f;
        totalScoreText.SetText("0");
        gameOverText.gameObject.SetActive(false);
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

    // Invoke specific actions depending on the game state.
    public void DoWork () 
    {
        // If the game is busy with animations, hold off on game state changes.
        if (_busyDuration > 0f)
        {
            tileSwapper.Update();
            _busyDuration -= Time.deltaTime;
            if (_busyDuration > 0f) return;
        }

        if (game.HasMatches) ProcessMatches();
        else if (game.NeedsFilling) DropTiles();
        else if (!IsPlaying) gameOverText.gameObject.SetActive(true);
    }

    // Invokes Match3Game's ProcessMatches and makes all cleared tiles disappear.
    private void ProcessMatches ()
    {
        game.ProcessMatches();

        for (int i = 0; i < game.ClearedTileCoordinates.Count; i++)
        {
            int2 c = game.ClearedTileCoordinates[i];
            _busyDuration = Mathf.Max(_tiles[c].Disappear(), _busyDuration);
            _tiles[c] = null;
        }

        totalScoreText.SetText("{0}", game.TotalScore);

        for (int i = 0; i < game.Scores.Count; i++)
        {
            SingleScore score = game.Scores[i];
            floatingScorePrefab.Show(
                new Vector3(
                    score.position.x + _tileOffset.x, 
                    score.position.y + _tileOffset.y,
                    _floatingScoreZ
                ), 
                score.value
            );

            // Avoids weird overlapping of floating scores.
            _floatingScoreZ = _floatingScoreZ <= -0.02f ? 0f : _floatingScoreZ - 0.001f;
        }
    }

    // Invokes Match3Game's DropTiles method and handles fallen + new tiles.
    private void DropTiles()
    {
        game.DropTiles();

        for (int i = 0; i < game.DroppedTiles.Count; i++)
        {
            TileDrop drop = game.DroppedTiles[i];
            Tile tile;

            // If the given tile fell, adjust its position.
            if (drop.fromY < _tiles.SizeY)
            {
                tile = _tiles[drop.coordinates.x, drop.fromY];
            }
            else // If not, spawn a new tile.
            {
                tile = SpawnTile(game[drop.coordinates], drop.coordinates.x, drop.fromY + newDropOffset);
            }

            _tiles[drop.coordinates] = tile;
            _busyDuration = Mathf.Max(tile.Fall(drop.coordinates.y + _tileOffset.y, dropSpeed), _busyDuration);
        }
    }

    // Handles dragging input using touch.
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
        // Calls tileSwapper to handle tile swapping + animation.
        bool success = game.TryMove(move);
        Tile a = _tiles[move.From], b = _tiles[move.To];
        _busyDuration = tileSwapper.Swap(a, b, !success);

        if (success)
        {
            _tiles[move.From] = b;
            _tiles[move.To] = a;
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
