using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
/*
 * This script uses the Input Asset and its generated C# script to take in player input.
 * It calls events when touch is registered, which calls all methods subscribed to them in other scripts.
 * This script is a singleton.
 */


// Ensures that this script runs before all others
[DefaultExecutionOrder(-1)]
public class InputManager : MonoBehaviour
{
    // Start and End Touch delegates and events
    public delegate void StartTouchEvent(Vector3 position, float time, InputAction.CallbackContext context);
    public event StartTouchEvent OnStartTouch;
    public delegate void EndTouchEvent(Vector3 position, float time, InputAction.CallbackContext context);
    public event EndTouchEvent OnEndTouch;

    // Debug inputs delegates and events
    public delegate void StartRestartEvent(float isPressed, InputAction.CallbackContext context);
    public event StartRestartEvent OnStartRestart;
    public delegate void StartMouseTouchEvent(Vector3 position, float time, InputAction.CallbackContext context);
    public event StartMouseTouchEvent OnStartMouseTouch;
    public delegate void EndMouseTouchEvent(Vector3 position, float time, InputAction.CallbackContext context);
    public event EndMouseTouchEvent OnEndMouseTouch;

    // Input Asset script
    private TouchControls _touchControls;

    // Singleton instance
    public static InputManager instance;

    private Camera _mainCamera;

	private void Awake()
	{
        // check if instance exists
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        _touchControls = new TouchControls();

        _mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        _touchControls.Enable();
    }

    private void OnDisable()
    {
        _touchControls?.Disable();
    }

    // Register touch input
    private void Start()
    {
        _touchControls.Touch.TouchPress.started += ctx => StartTouch(ctx);
        _touchControls.Touch.TouchPress.canceled += ctx => EndTouch(ctx);

        // Debug input for mouse and restart
        _touchControls.Debug.TouchPress.started += ctx => StartTouch(ctx);
        _touchControls.Debug.TouchPress.canceled += ctx => EndTouch(ctx);

        _touchControls.Debug.RestartGame.started += ctx => RestartGame(ctx);

    }

    // using New Input System
    // if touch or mouse is read, every method subscribed to OnStartTouch or OnStartMouseTouch gets called
    private void StartTouch(InputAction.CallbackContext context)
    {
        if (context.control == Mouse.current.leftButton)
        {
            if (OnStartMouseTouch != null) OnStartMouseTouch(_touchControls.Debug.TouchPosition.ReadValue<Vector2>(), (float)context.startTime, context);
            Debug.Log("Mouse touch started");
        }
        else if (context.control == Touchscreen.current.primaryTouch.press)
        {
            if (OnStartTouch != null) OnStartTouch(_touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.startTime, context);
        }
        else
        {
            Debug.LogWarning("Input went wrong");
        }
    }

    // if touch or mouse stops, every method subscribed to OnEndTouch or OnEndMouseTouch gets called
    private void EndTouch(InputAction.CallbackContext context)
    {
        if (context.control == Mouse.current.leftButton)
        {
            if (OnEndMouseTouch != null) OnEndMouseTouch(_touchControls.Debug.TouchPosition.ReadValue<Vector2>(), (float)context.time, context);
            Debug.Log("Mouse touch ended");
        }
        else if (context.control == Touchscreen.current.primaryTouch.press)
        {
            if (OnEndTouch != null) OnEndTouch(_touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.time, context);
        }
        else
        {
            Debug.LogWarning("Input end went wrong");
        }
    }

    // DEBUG ONLY: if space bar is pressed, the game restarts
    private void RestartGame(InputAction.CallbackContext context)
    {
        if (OnStartRestart != null) OnStartRestart(_touchControls.Debug.RestartGame.ReadValue<float>(), context);
    }

    public int OnPress(InputAction.CallbackContext context, GameObject target)
    {
        if (!context.started) return 0;

        var rayHit = Physics2D.GetRayIntersection(_mainCamera.ScreenPointToRay(_touchControls.Touch.TouchPosition.ReadValue<Vector3>()));
        if (!rayHit.collider) return 0;

        if(rayHit.collider.gameObject == target) return 1;

        return -1;
    }

    // Convert screen coordinates to world coordinates
    public static Vector3 ScreenToWorld(Camera camera, Vector3 position)
    {
        position.z = 0;
        return camera.ScreenToWorldPoint(position);
    }

    // currently not in use
    public Vector2 PrimaryPosition()
    {
        return ScreenToWorld(_mainCamera, _touchControls.Touch.TouchPosition.ReadValue<Vector3>());
    }
}
