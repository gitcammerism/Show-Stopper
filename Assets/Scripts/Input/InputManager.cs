using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

// ensure that this script runs before all others
[DefaultExecutionOrder(-1)]
public class InputManager : MonoBehaviour
{
    // events for other scripts to call
    public delegate void StartTouchEvent(Vector2 position, float time);
    public event StartTouchEvent OnStartTouch;
    public delegate void EndTouchEvent(Vector2 position, float time);
    public event EndTouchEvent OnEndTouch;

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
        TouchSimulation.Enable();

        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += FingerDown;
    }

    private void OnDisable()
    {
        _touchControls?.Disable();
        TouchSimulation.Disable();

        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= FingerDown;
    }

    private void Start()
    {
        _touchControls.Touch.TouchPress.started += ctx => StartTouch(ctx);
        _touchControls.Touch.TouchPress.canceled += ctx => EndTouch(ctx);

    }

    // using New Input System
    private void StartTouch(InputAction.CallbackContext context)
    {
        //Vector2 touchPosition = _touchControls.Touch.TouchPosition.ReadValue<Vector2>();
        //Debug.Log($"Touch started at position: {touchPosition}");
        Debug.Log("Touch started " + _touchControls.Touch.TouchPosition.ReadValue<Vector2>()); // will return position of touch in screen coords

        if (OnStartTouch != null) OnStartTouch(_touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.startTime);
    }

    private void EndTouch(InputAction.CallbackContext context)
    {
        //Vector2 touchPosition = _touchControls.Touch.TouchPosition.ReadValue<Vector2>();
        //Debug.Log($"Touch ended at position: {touchPosition}");
        if (OnEndTouch != null) OnEndTouch(_touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.time);
    }

    // using EnhancedTouch system
    private void FingerDown(Finger finger)
    {
        if (OnStartTouch != null) OnStartTouch(finger.screenPosition, Time.time);
    }

    public void OnPress(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        var rayHit = Physics2D.GetRayIntersection(_mainCamera.ScreenPointToRay(_touchControls.Touch.TouchPosition.ReadValue<Vector2>()));
        if (!rayHit.collider) return;

        Debug.Log(rayHit.collider.gameObject.name);
    }
}
