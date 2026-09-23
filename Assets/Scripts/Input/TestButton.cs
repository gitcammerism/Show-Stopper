using UnityEngine;
using UnityEngine.InputSystem;

// THIS IS AN EXAMPLE SCRIPT FOR HOW TO APPLY INPUT SYSTEM TO OBJECTS
public class TestButton : MonoBehaviour
{
    // important note to document somewhere (TDD) : when subscribing to touch event, whatever function it calls needs to take Vector2 and float params
    
    private InputManager _inputManager;

    private void Awake()
    {
        _inputManager = InputManager.instance;
    }

    private void OnEnable()
    {
        // subscribe to touch event
        _inputManager.OnStartTouch += Press;
    }

    private void OnDisable()
    {
        // unsubscribe from touch event
        _inputManager.OnEndTouch -= Press;
    }

    // Press gets called whenever the OnStartTouch event is called
    public void Press(Vector2 screenPosition, float time, InputAction.CallbackContext context)
    {
        //Vector3 screenCoords = new Vector3(screenPosition.x, screenPosition.y, 0);

        if(_inputManager.OnPress(context, this.gameObject) == 1)
        {
            Debug.Log("successful Press");
            // get some actual logic
        }
        else
        {
            Debug.Log("Press fail");
        }
    }

}
