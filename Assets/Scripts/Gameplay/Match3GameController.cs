using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

//This script controls the game start and end, taking in player input and processing it through Match3Skin.

public class Match3GameController : MonoBehaviour
{
    [SerializeField] private Match3Skin match3;
    [SerializeField] private TextMeshProUGUI gameOverText;

    // Input-handling variables
    private Vector3 _dragStart;
    private bool _isDragging;
    private InputManager _inputManager;
    private bool _isGameOver = false;

    // Once this game object awakens, it starts a new game via Match3Skin.
    private void Awake()
    {
        _inputManager = InputManager.instance;
        gameOverText.gameObject.SetActive(false);
        match3.StartNewGame();
    }

    // Subscribes to the input events when the game object is enabled.
    private void OnEnable()
    {
        if(InputManager.instance != null)
        {
            _inputManager.OnStartTouch += TouchStarted;
            _inputManager.OnEndTouch += TouchEnded;
            _inputManager.OnStartRestart += GameRestarted;
        }
    }

    // Unsubscribes from the input events when the game object is disabled.
    private void OnDisable()
    {
        if(InputManager.instance != null)
        {
            InputManager.instance.OnStartTouch -= TouchStarted;
            InputManager.instance.OnEndTouch -= TouchEnded;
        }
    }

    // Checks if the game is ongoing.
    void Update ()
    {
        if (match3.IsPlaying) match3.DoWork();
        else if (!_isGameOver)
        {
            _isGameOver = true;
            gameOverText.gameObject.SetActive(true);
            Debug.Log("Game is over.");
        }
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
}
