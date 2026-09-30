using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

//This script controls the game start and end, taking in player input and processing it through Match3Skin.

public class Match3GameController : MonoBehaviour
{
    [SerializeField] private Match3Skin match3;

    // Input-handling variables
    private Vector3 _dragStart;
    private bool _isDragging;
    private InputManager _inputManager;


    // Once this game object awakens, it starts a new game via Match3Skin.
    private void Awake()
    {
        _inputManager = InputManager.instance;
        match3.StartNewGame();
    }

    // Subscribes to the input events when the game object is enabled.
    private void OnEnable()
    {
        if(InputManager.instance != null)
        {
            _inputManager.OnStartTouch += TouchStarted;
            _inputManager.OnEndTouch += TouchEnded;
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

    // Checks if the game is ongoing and handles input if the game is not busy.
    void Update ()
    {
        if (match3.IsPlaying) match3.DoWork();

        else if (Input.GetKeyDown(KeyCode.Space)) // temp debug key to restart game
        {
            match3.StartNewGame();
        }
    }

    // Subscribes to OnStartTouch, checks if player is currently dragging a tile
    private void TouchStarted(Vector3 position, float time, InputAction.CallbackContext context)
    {
        if (!_isDragging && !match3.IsBusy)
        {
            Debug.Log("TouchStarted: " + position);
            _dragStart = position;
            _isDragging = true;
        }
    }
    
    private void TouchEnded(Vector3 position, float time, InputAction.CallbackContext context)
    {
        if (_isDragging && !match3.IsBusy)
        {
            Debug.Log("TouchEnded: " + position);
            match3.EvaluateDrag(_dragStart, position);
            _isDragging = false;
        }
    }
}
