using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

//This script controls the game start and end, taking in player input and processing it through Match3Skin.

public class Match3GameController : MonoBehaviour
{
    [SerializeField] private Match3Skin match3;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI gameWinText;

    // Input-handling variables
    private Vector3 _dragStart;
    private bool _isDragging;
    private InputManager _inputManager;

    public static event Action StopTimer;

    // Once this game object awakens, it starts a new game via Match3Skin.
    private void Awake()
    {
        _inputManager = InputManager.instance;
        gameOverText.gameObject.SetActive(false);
        gameWinText.gameObject.SetActive(false);
        match3.StartNewGame();
    }

    // Subscribes to  events when the game object is enabled.
    private void OnEnable()
    {
        // Subscribes to input events.
        if(InputManager.instance != null)
        {
            _inputManager.OnStartTouch += TouchStarted;
            _inputManager.OnEndTouch += TouchEnded;
            _inputManager.OnStartRestart += GameRestarted;
        }

        // Subscribes to win & lose condition events.
        Match3Skin.OnGameLost += GameOver;
        Match3Skin.OnGameWon += GameWin;
        ResourceManager.OnGoalReached += GameWinNotify;
    }

    // Unsubscribes from events when the game object is disabled.
    private void OnDisable()
    {
        // Unsubscribes from input events.
        if(InputManager.instance != null)
        {
            InputManager.instance.OnStartTouch -= TouchStarted;
            InputManager.instance.OnEndTouch -= TouchEnded;
        }
        
        // Unsubscribes to win & lose condition events.
        Match3Skin.OnGameLost -= GameOver;
        Match3Skin.OnGameWon -= GameWin;
        ResourceManager.OnGoalReached -= GameWinNotify;
    }

    // Calls on Match3Skin's DoWork() every frame.
    void Update ()
    {
        if (match3.IsPlaying) match3.DoWork();
    }

    // Subscribes to OnStartTouch, checks if player is currently dragging a tile
    private void TouchStarted(Vector3 position, float time, InputAction.CallbackContext context)
    {
        if (!_isDragging && !match3.IsBusy)
        {
            _dragStart = position;
            _isDragging = true;
        }
    }
    
    // Subscribes to OnEndTouch, checks if player has lifted finger from screen
    private void TouchEnded(Vector3 position, float time, InputAction.CallbackContext context)
    {
        if (_isDragging && !match3.IsBusy)
        {
            match3.EvaluateDrag(_dragStart, position);
            _isDragging = false;
        }
    }

    // temp debug key spacebar to restart game
    private void GameRestarted(float isPressed, InputAction.CallbackContext context)
    {
        if (isPressed > 0.5f) match3.StartNewGame();
    }

    // Handles the loss of a level.
    private void GameOver()
    {
        gameOverText.gameObject.SetActive(true);
    }

    // Notifies Match3Skin that the game should end, then stops the timer.
    private void GameWinNotify()
    {
        StopTimer?.Invoke();
        match3.GameOverNotify(true);
    }

    // Handles the winning of a level.
    private void GameWin()
    {
        Debug.Log("We reached the level goal!");
        gameWinText.gameObject.SetActive(true);
    }
}
