using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
	private TouchControls _touchControls;
    // Singleton instance
    public static InputManager instance;
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

    private void StartTouch(InputAction.CallbackContext context)
    {
        //Vector2 touchPosition = _touchControls.Touch.TouchPosition.ReadValue<Vector2>();
        //Debug.Log($"Touch started at position: {touchPosition}");
        Debug.Log("Touch started " + _touchControls.Touch.TouchPosition.ReadValue<Vector2>()); // will return position of touch in screen coords
    }

    private void EndTouch(InputAction.CallbackContext context)
    {
        Vector2 touchPosition = _touchControls.Touch.TouchPosition.ReadValue<Vector2>();
        Debug.Log($"Touch ended at position: {touchPosition}");
    }
}
