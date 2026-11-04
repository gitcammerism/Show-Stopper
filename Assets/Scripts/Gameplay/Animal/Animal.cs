using System;
using UnityEngine;

public class Animal : MonoBehaviour
{
    InputManager inputManager = InputManager.instance;

    

    private void OnEnable()
    {
        if (inputManager != null)
        {
            inputManager.OnStartTouch += OnTouch;
        }
    }

    private void OnTouch(Vector3 position, float time, UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        // zoom in on animal here
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
}
