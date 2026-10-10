using System;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

// This script tracks & handles the current state of the game and starts a new game when prompted by Match3GameController.

public class Match3Skin : MonoBehaviour
{
    // Game setup.
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private Match3Game game;
    [SerializeField] private Tile[] tilePrefabs;
    [SerializeField] private FloatingScore floatingScorePrefab;
    private float _floatingScoreZ;
    private MatchGrid2D<Tile> _tiles;
    private float2 _tileOffset;
    
    public GridShape gridShape = GridShape.Square;

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

    [System.NonSerialized]
    public bool gameOver;
    public static event Action OnGameLost;
    public static event Action OnGameWon;
       
    public bool IsPlaying => (IsBusy || game.PossibleMove.IsValid) && (!gameOver || !game.PossibleMove.IsValid);
    public bool IsBusy => _busyDuration > 0f;

    // Starts a new game by calling Match3Game.StartNewGame().
    public void StartNewGame ()
    {
        TimeManager.OnTimerEnd += GameOverNotify;
        _busyDuration = 0f;
        totalScoreText.SetText("0");
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

        switch (gridShape)
        {
            case GridShape.Circle:
                SpawnTilesCircle();
                break;
            
            case GridShape.Heart:
                SpawnTilesHeart();
                break;
            
            case GridShape.Square:
            default:
                SpawnTilesSquare();
                break;
        }
    }

    private void SpawnTilesSquare()
    {
        // Loops through tile grid and spawns tiles.
        for (int y = 0; y < _tiles.SizeY; y++)
        {
            for (int x = 0; x < _tiles.SizeX; x++)
            {
                _tiles[x, y] = SpawnTile(game[x, y], x, y);
            }
        }
    }

    private void SpawnTilesCircle()
    {
        // World scale is 1, 1, 1 for tiles
        float tileWidth = 1.0f;  
        float tileHeight = 1.0f;

        // Calculate center indices to determine the circular radius boundary
        float centerX = (_tiles.SizeX - 1) * 0.5f;
        float centerY = (_tiles.SizeY - 1) * 0.5f;
        float gridRadius = Mathf.Min(_tiles.SizeX, _tiles.SizeY) * 0.5f;

        for (int y = 0; y < _tiles.SizeY; y++)
        {
            for (int x = 0; x < _tiles.SizeX; x++)
            {
                // Check if this grid coordinate falls outside the circle boundary
                float distanceFromCenter = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                if (distanceFromCenter > gridRadius)
                {
                    _tiles[x, y] = null; // Mark as empty slot if needed
                    continue; 
                }

                // Pass raw grid coordinates (x, y) directly. 
                // Your SpawnTile method will apply the _tileOffset automatically.
                _tiles[x, y] = SpawnTile(game[x, y], x * tileWidth, y * tileHeight);
            }
        }
    }

    private void SpawnTilesHeart()
    {
        float tileWidth = 1.0f;  
        float tileHeight = 1.0f;

        for (int y = 0; y < _tiles.SizeY; y++)
        {
            for (int x = 0; x < _tiles.SizeX; x++)
            {
                // Skip coordinates that fall outside the heart shape
                if (!game.IsInsideHeart(x, y))
                {
                    _tiles[x, y] = null;
                    continue; 
                }

                _tiles[x, y] = SpawnTile(game[x, y], x * tileWidth, y * tileHeight);
            }
        }
    }

    // Spawns a random tile at the given location.
    private Tile SpawnTile (TileState t, float x, float y) =>
        tilePrefabs[(int)t - 1].Spawn(new Vector3(x + _tileOffset.x, y + _tileOffset.y));
    
    private Tile SpawnTileCircle (TileState t, float x, float y) =>
        tilePrefabs[(int)t - 1].Spawn(new Vector3(x, y));

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
        
        if (gameOver && game.NeedsFilling) DropTiles();
    }

    // Invokes Match3Game's DropTiles method and handles fallen + new tiles.
    private void DropTiles()
    {
        switch (gridShape)
        {
            case GridShape.Circle:
                game.DropTilesCircle();
                DropTilesCircle();
                break;
            
            case GridShape.Heart:
                game.DropTilesHeart();
                DropTilesHeart();
                break;
            
            case GridShape.Square:
            default:
                game.DropTiles();
                DropTilesSquare();
                break;
        }
    }

    private void DropTilesSquare()
    {
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
    
    private void DropTilesCircle()
    {
        float centerX = (_tiles.SizeX - 1) * 0.5f;
        float centerY = (_tiles.SizeY - 1) * 0.5f;
        float gridRadius = Mathf.Min(_tiles.SizeX, _tiles.SizeY) * 0.5f;

        for (int i = 0; i < game.DroppedTiles.Count; i++)
        {
            TileDrop drop = game.DroppedTiles[i];
            int x = drop.coordinates.x;
            int y = drop.coordinates.y;

            // Skip out-of-bounds drops
            if (Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY)) > gridRadius) 
                continue;

            Tile tile;

            bool isFromValidSlot = drop.fromY < _tiles.SizeY && 
                                   Vector2.Distance(new Vector2(x, drop.fromY), new Vector2(centerX, centerY)) <= gridRadius;

            if (isFromValidSlot)
            {
                tile = _tiles[x, drop.fromY];
                _tiles[x, drop.fromY] = null;
            }
            else 
            {
                tile = SpawnTile(game[drop.coordinates], x, drop.fromY + newDropOffset);
            }

            _tiles[drop.coordinates] = tile;
            _busyDuration = Mathf.Max(tile.Fall(y + _tileOffset.y, dropSpeed), _busyDuration);
        }
    }

    private void DropTilesHeart()
    {
        for (int i = 0; i < game.DroppedTiles.Count; i++)
        {
            TileDrop drop = game.DroppedTiles[i];
            int x = drop.coordinates.x;
            int y = drop.coordinates.y;

            // Hard guard: never process or render anything outside the heart
            if (!game.IsInsideHeart(x, y)) continue;

            Tile tile;
            bool isFromValidSlot = drop.fromY < _tiles.SizeY && game.IsInsideHeart(x, drop.fromY);

            if (isFromValidSlot && _tiles[x, drop.fromY] != null)
            {
                tile = _tiles[x, drop.fromY];
                _tiles[x, drop.fromY] = null;
            }
            else 
            {
                tile = SpawnTile(game[drop.coordinates], x, drop.fromY + newDropOffset);
            }

            _tiles[x, y] = tile;
            _busyDuration = Mathf.Max(tile.Fall(y + _tileOffset.y, dropSpeed), _busyDuration);
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

    // If not busy, end the game or wait for game to end.
    public void GameOverNotify(bool playerWon)
    {
        if (!IsBusy)
        {
            if (playerWon) OnGameWon?.Invoke();
            else  OnGameLost?.Invoke();
            gameOver = true;
        }
        else StartCoroutine(WaitForBusyEnd(playerWon));
    }

    // Waits for the game to not be busy, then ends the game.
    private IEnumerator WaitForBusyEnd(bool playerWon)
    {
        while (IsBusy) yield return null;
        if (!playerWon)
        {
            OnGameLost?.Invoke();
            gameOver = true;
        }
        else OnGameWon?.Invoke();
    }
}
