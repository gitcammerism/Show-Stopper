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
    public Grid gameGrid;
    private Match3Board _board;
    [SerializeField] private TextMeshProUGUI totalScoreText;
    // FROM COLIN - For Highscore display text
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Match3GameController gameController;
    [SerializeField] private Match3Game game;
    [SerializeField] private FloatingScore floatingScorePrefab;
    [SerializeField] private ParticleSystem matchParticles;
    private float _floatingScoreZ;
    private MatchGrid2D<OldTile> _tiles;
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

    [System.NonSerialized]
    public bool gameOver;
    public static event Action OnGameLost;
    public static event Action OnGameWon;
       
    public bool IsPlaying => (IsBusy || game.PossibleMove.IsValid) && (!gameOver || !game.PossibleMove.IsValid);
    public bool IsBusy => _busyDuration > 0f;

    // Starts a new game by calling Match3Game.StartNewGame().
    public void StartNewGame ()
    {
        _board = gameGrid.GetComponent<Match3Board>();
        TimeManager.OnTimerEnd += GameOverNotify;
        _busyDuration = 0f;
        totalScoreText.SetText("0");

        // FROM COLIN - Write down Level High Score
        int highScore = DataPersistenceManager.instance.GetLevelHighScore(gameController.LevelID);
        highScoreText.SetText("High Score: {0}", highScore);

        _board.StartNewGame();
    }
    
    // Handles dragging input using touch.
    public bool EvaluateDrag (Vector3 start, Vector3 end)
    {
        // Determines the tile position of a & b then creates a Move struct based on the move performed.
        Vector3 a = _board.ScreenToTileSpace(start), b = _board.ScreenToTileSpace(end);
        Vector3 swipe = _board.ScreenToTileSpace(b - a);

        if (swipe == Vector3.zero) return true;
        MoveDirection direction;
        
        // Determines the direction of the swipe.
        if (Mathf.Abs(swipe.x) >= Mathf.Abs(swipe.y))
        {
            // Horizontal swipe
            direction = swipe.x > 0 ? MoveDirection.Right : MoveDirection.Left;
        }
        else
        {
            // Vertical swipe
            direction = swipe.y > 0 ? MoveDirection.Up : MoveDirection.Down;
        }
        
        Move move = new Move(a, swipe, direction);
        if (move.To == move.From) return true;

        // If the move is valid and the new coordinates are valid, the tiles can be moved.
        if (move.IsValid && _board.IsValidTile(move.From) && _board.IsValidTile(move.To))
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
        bool success = _board.TryMove(move);
        Tile a = _board._boardTiles[_board.ScreenToTileSpace(move.From)];
        Tile b = _board._boardTiles[_board.ScreenToTileSpace(move.To)];
        _busyDuration = tileSwapper.Swap(a, b, !success);

        if (success)
        {
            _board._boardTiles[_board.ScreenToTileSpace(move.From)] = b;
            _board._boardTiles[_board.ScreenToTileSpace(move.To)] = a;
        }
    }

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

        if (_board.HasMatches()) ProcessMatches();
        //else if (game.NeedsFilling) DropTiles();

        // Move result = Move.FindMove(game);
        // if (result.Equals(default(Move)))
        // {
        //     ResetGame();
        // }
    }

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

    // Invokes Match3Board's ProcessMatches and makes all cleared tiles disappear.
    private void ProcessMatches ()
    {
        _board.ProcessMatches();

        for (int i = 0; i < _board.ClearedTiles.Count; i++)
        {
            Vector3 c = _board.ClearedTiles[i];
            _busyDuration = Mathf.Max(_board._boardTiles[c].Disappear(), _busyDuration);
            _board._boardTiles[c] = null;
        }

        totalScoreText.SetText("{0}", game.TotalScore);

        for (int i = 0; i < _board.Scores.Count; i++)
        {
            SingleScore score = _board.Scores[i];
            matchParticles.transform.position = new Vector3(
                score.position.x + _tileOffset.x, 
                score.position.y + _tileOffset.y, 
                0f);
            matchParticles.Play();

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

        if (gameOver && _board.NeedsFilling) DropTiles();
    }

    // Invokes Match3Board's DropTiles method and handles fallen + new tiles.
    private void DropTiles()
    {
        for (int i = 0; i < _board.DroppedTiles.Count; i++)
        {
            TileDrop drop = _board.DroppedTiles[i];
            Tile tile;

            // If the given tile fell, adjust its position.
            if (drop.fromY < _tiles.SizeY)
            {
                tile = _board._boardTiles[drop.coordinates.x, drop.fromY];
            }
            else // If not, spawn a new tile.
            {
                tile = SpawnTile(game[drop.coordinates], drop.coordinates.x, drop.fromY + newDropOffset);
            }

            _tiles[drop.coordinates] = tile;
            _busyDuration = Mathf.Max(tile.Fall(drop.coordinates.y + _tileOffset.y, dropSpeed), _busyDuration);
        }
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
