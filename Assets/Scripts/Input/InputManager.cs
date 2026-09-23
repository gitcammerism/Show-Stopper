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
    public delegate void StartTouchEvent(Vector2 position, float time, InputAction.CallbackContext context);
    public event StartTouchEvent OnStartTouch;
    public delegate void EndTouchEvent(Vector2 position, float time, InputAction.CallbackContext context);
    public event EndTouchEvent OnEndTouch;

    private TouchControls _touchControls;
    // Singleton instance
    public static InputManager instance;

    private Camera _mainCamera;

    public GameObject targetObject;
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

    private void Start()
    {
        _touchControls.Touch.TouchPress.started += ctx => StartTouch(ctx);
        _touchControls.Touch.TouchPress.canceled += ctx => EndTouch(ctx);

    }

    // using New Input System
    private void StartTouch(InputAction.CallbackContext context)
    {
        //Debug.Log("Touch started " + _touchControls.Touch.TouchPosition.ReadValue<Vector2>()); // will return position of touch in screen coords

        // if touch is read, every method subscribed to OnStartTouch gets called
        if (OnStartTouch != null) OnStartTouch(_touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.startTime, context);
    }

    private void EndTouch(InputAction.CallbackContext context)
    {
        // if touch stops, every method subscribed to OnEndTouch gets called
        if (OnEndTouch != null) OnEndTouch(_touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.time, context);
    }

    public int OnPress(InputAction.CallbackContext context, GameObject target)
    {
        if (!context.started) return 0;

        var rayHit = Physics2D.GetRayIntersection(_mainCamera.ScreenPointToRay(_touchControls.Touch.TouchPosition.ReadValue<Vector2>()));
        if (!rayHit.collider) return 0;

        if(rayHit.collider.gameObject == target) return 1;

        return -1;

        //Debug.Log(rayHit.collider.gameObject.name);
    }

    // Convert screen coordinates to world coordinates
    public static Vector3 ScreenToWorld(Camera camera, Vector3 position)
    {
        position.z = 0;
        return camera.ScreenToWorldPoint(position);
    }

    //// using EnhancedTouch system
    //private void FingerDown(Finger finger)
    //{
    //    if (OnStartTouch != null) OnStartTouch(finger.screenPosition, Time.time);
    //}
}
