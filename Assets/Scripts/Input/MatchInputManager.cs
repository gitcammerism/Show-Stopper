using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Tilemaps;

public class MatchInputManager : MonoBehaviour
{
    [SerializeField] private Tilemap tileMap;
    private Camera mainCamera;
    public Match3Game game;
    public LayerMask tilemapLayerMask;
    private Vector3Int selectedCell;

    private Vector2 startPosition;
    private Vector2 endPosition;
    private TileState selectedTile;
    
    // Minimum distance in pixels to count as swipe deadzone
    [SerializeField] private float swipeThreshold = 20f;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // detect initial input
        if (PointerPressed()) OnPressStart();
        
        // detect release input
        //if (PointerReleased()) OnPressEnd();
    }

    private bool PointerPressed()
    {
        if ((Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) ||
            (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)) return true;
        return false;
    }

    private bool PointerReleased()
    {
        if ((Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame) ||
            (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)) return true;
        return false;
    }

    private Vector2 GetPointerPosition()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }

        if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();
        }
        
        return Vector2.zero;
    }

    private void OnPressStart()
    {
        startPosition = GetPointerPosition();
        
        // Convert screen point to world raycast to see if player clicked a tile
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(startPosition);
        
        // Convert world space coordinates to cell index coordinates
        //selectedCell = tileMap.Cell
        
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, Mathf.Infinity, tilemapLayerMask);
        
        //if (hit.collider != null && hit.collider.TryGetComponent<Tile>(out Tile tile))
    }
    
}
