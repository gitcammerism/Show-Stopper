using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeDetection : MonoBehaviour
{
    private InputManager _inputManager;

    private Vector2 _startPosition;
    private float _startTime;
    private Vector2 _endPosition;
    private float _endTime;

    [SerializeField, Range(0, 1)]
    private float _directionThreshold = 0.9f;

    private void Awake()
    {
        _inputManager = InputManager.instance;
    }

    private void OnEnable()
    {
        _inputManager.OnStartTouch += SwipeStart;
        _inputManager.OnEndTouch += SwipeEnd;
    }

    private void OnDisable()
    {
        _inputManager.OnEndTouch -= SwipeStart;
        _inputManager.OnEndTouch -= SwipeEnd;
    }

    private void SwipeStart(Vector2 position, float time, InputAction.CallbackContext context)
    {
        _startPosition = position;
        _startTime = time;
    }

    private void SwipeEnd(Vector2 position, float time, InputAction.CallbackContext context)
    {
        _endPosition = position;
        _endTime = time;
        DetectSwipe();
    }

    // Handles calculating direction vectors and calls SwipeDirection
    private void DetectSwipe()
    {
        Debug.DrawLine(_startPosition, _endPosition, Color.red, 5f);
        Vector3 direction = _endPosition - _startPosition;
        Vector2 direction2D = new Vector2(direction.x, direction.y).normalized;
        SwipeDirection(direction2D);
    }

    // Determines which cardinal direction the swipe is pointing
    // If user swipes at an angle, the swipe will be processed in the nearest cardinal direction
    // Priority is given in chronological order
    private void SwipeDirection(Vector2 direction)
    {
        if(Vector2.Dot(Vector2.up, direction) > _directionThreshold)
        {
            //Debug.Log("Swipe up");
        }
        else if (Vector2.Dot(Vector2.down, direction) > _directionThreshold)
        {
            //Debug.Log("Swipe down");
        }
        else if (Vector2.Dot(Vector2.left, direction) > _directionThreshold)
        {
            //Debug.Log("Swipe left");
        }
        else if (Vector2.Dot(Vector2.right, direction) > _directionThreshold)
        {
            //Debug.Log("Swipe right");
        }
    }
}
