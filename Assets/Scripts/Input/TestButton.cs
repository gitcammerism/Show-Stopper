using UnityEngine;

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
        _inputManager.OnStartTouch += Press;
    }

    private void OnDisable()
    {
        _inputManager.OnEndTouch -= Press;
    }

    public void Press(Vector2 screenPosition, float time)
    {
        Vector3 screenCoords = new Vector3(screenPosition.x, screenPosition.y, 0);

    }
}
