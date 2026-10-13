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
    // FROM COLIN - For Highscore display text
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Match3GameController gameController;
    [SerializeField] private Match3Game game;
    [SerializeField] private OldTile[] tilePrefabs;
    [SerializeField] private FloatingScore floatingScorePrefab;
    [SerializeField] private ParticleSystem matchParticles;
    private float _floatingScoreZ;
    private MatchGrid2D<OldTile> _tiles;
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

        // FROM COLIN - Write down Level High Score
        int highScore = DataPersistenceManager.instance.GetLevelHighScore(gameController.LevelID);
        highScoreText.SetText("High Score: {0}", highScore);

        game.StartNewGame();
    }

    // Spawns a random tile at the given location.
    private OldTile SpawnTile (TileType t, float x, float y)
    {
        if (t == TileType.None) return null;
        return tilePrefabs[(int)t - 1].Spawn(new Vector3(x + _tileOffset.x, y + _tileOffset.y));
    }

    // Invoke specific actions depending on the game state.
    //public void DoWork () 
    //{
    //    // If the game is busy with animations, hold off on game state changes.
    //    if (_busyDuration > 0f)
    //    {
    //        tileSwapper.Update();
    //        _busyDuration -= Time.deltaTime;
    //        if (_busyDuration > 0f) return;
    //    }

    //    //if (game.HasMatches) ProcessMatches();
    //    //else if (game.NeedsFilling) DropTiles();

    //    Move result = Move.FindMove(game);
    //    if (result.Equals(default(Move)))
    //    {
    //        ResetGame();
    //    }
    //}

    //public void ResetGame()
    //{
    //    for (int y = 0; y < _tiles.SizeY; y++)
    //    {
    //        for (int x = 0; x < _tiles.SizeX; x++)
    //        {
    //            if (_tiles[x, y] == null) continue;
    //            _tiles[x, y].Despawn();
    //            _tiles[x, y] = null;
    //        }
    //    }
            
    //    game.ClearGrid();
    //}

    // Invokes Match3Game's ProcessMatches and makes all cleared tiles disappear.
    //private void ProcessMatches ()
    //{
    //    game.ProcessMatches();

    //    for (int i = 0; i < game.ClearedTileCoordinates.Count; i++)
    //    {
    //        int2 c = game.ClearedTileCoordinates[i];
    //        _busyDuration = Mathf.Max(_tiles[c].Disappear(), _busyDuration);
    //        _tiles[c] = null;
    //    }

    //    totalScoreText.SetText("{0}", game.TotalScore);

    //    for (int i = 0; i < game.Scores.Count; i++)
    //    {
    //        SingleScore score = game.Scores[i];
    //        matchParticles.transform.position = new Vector3(score.position.x + _tileOffset.x, score.position.y + _tileOffset.y, 0f);
    //        matchParticles.Play();

    //        floatingScorePrefab.Show(
    //            new Vector3(
    //                score.position.x + _tileOffset.x, 
    //                score.position.y + _tileOffset.y,
    //                _floatingScoreZ
    //            ), 
    //            score.value
    //        );

    //        // Avoids weird overlapping of floating scores.
    //        _floatingScoreZ = _floatingScoreZ <= -0.02f ? 0f : _floatingScoreZ - 0.001f;
    //    }

    //    if (gameOver && game.NeedsFilling) DropTiles();
    //}

    // Invokes Match3Game's DropTiles method and handles fallen + new tiles.
    //private void DropTilesSquare()
    //{
    //    for (int i = 0; i < game.DroppedTiles.Count; i++)
    //    {
    //        TileDrop drop = game.DroppedTiles[i];
    //        Tile tile;

    //        // If the given tile fell, adjust its position.
    //        if (drop.fromY < _tiles.SizeY)
    //        {
    //            tile = _tiles[drop.coordinates.x, drop.fromY];
    //        }
    //        else // If not, spawn a new tile.
    //        {
    //            tile = SpawnTile(game[drop.coordinates], drop.coordinates.x, drop.fromY + newDropOffset);
    //        }

    //        _tiles[drop.coordinates] = tile;
    //        _busyDuration = Mathf.Max(tile.Fall(drop.coordinates.y + _tileOffset.y, dropSpeed), _busyDuration);
    //    }
    //}

    // Handles dragging input using touch.
    //public bool EvaluateDrag (Vector3 start, Vector3 end)
    //{
    //    // Determines the tile position of a & b then creates a Move struct based on the move performed.
    //    float2 a = ScreenToTileSpace(start), b = ScreenToTileSpace(end);
    //    var move = new Move(
    //        (int2)floor(a), (b - a) switch
    //        {
    //            var d when d.x > _dragThreshold => MoveDirection.Right,
    //            var d when d.x < -_dragThreshold => MoveDirection.Left,
    //            var d when d.y > _dragThreshold => MoveDirection.Up,
    //            var d when d.y < -_dragThreshold => MoveDirection.Down,
    //            _ => MoveDirection.None
    //        }
    //    );

    //    // If the move is valid and the new coordinates are valid, the tiles can be moved.
    //    if (move.IsValid && _tiles.AreValidCoordinates(move.From) && _tiles.AreValidCoordinates(move.To))
    //    {
    //        // Aborts the move if either tile is an obstacle or none.
    //        if (game[move.From] == TileType.Obstacle || game[move.To] == TileType.Obstacle || game[move.From] == TileType.None || game[move.To] == TileType.None) return true;

    //        DoMove(move);
    //        return false;
    //    }
    //    return true;
    //}

    //// Attempts the given move and swaps both tiles if successful.
    //private void DoMove (Move move)
    //{
    //    // Calls tileSwapper to handle tile swapping + animation.
    //    bool success = game.TryMove(move);
    //    Tile a = _tiles[move.From], b = _tiles[move.To];
    //    _busyDuration = tileSwapper.Swap(a, b, !success);

    //    if (success)
    //    {
    //        _tiles[move.From] = b;
    //        _tiles[move.To] = a;
    //    }
    //}

    //// Converts the given screen space coordinations to the actual tile coordinates.
    //private float2 ScreenToTileSpace (Vector3 screenPosition)
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(screenPosition);
    //    Vector3 p = ray.origin - ray.direction * (ray.origin.z / ray.direction.z);
    //    return float2(p.x - _tileOffset.x + 0.5f, p.y - _tileOffset.y + 0.5f);
    //}

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
