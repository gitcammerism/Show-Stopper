using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// handles dragging input for the game board

public class BoardInput : MonoBehaviour
{
    [SerializeField] private BoardBuilder board;
    [SerializeField] private Camera cam;
    [SerializeField] private float swipeThreshold = 0.35f;

    public bool inputLocked;

    private Cell startCell;
    private Vector3 startWorld;
    private bool dragging;

    void Update()
    {
        var p = Pointer.current;
        if (p == null || inputLocked)
        {
            dragging = false;
            return;
        }

        Vector2 pos = p.position.ReadValue();

        if (!dragging && p.press.wasPressedThisFrame) BeginDrag(pos);
        else if (dragging && p.press.isPressed) UpdateDrag(pos);
        else if (p.press.wasReleasedThisFrame) dragging = false;
    }

    // converts screen pos to world pos
    private Vector3 ToWorld(Vector2 screenPos)
    {
        return cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -cam.transform.position.z));
    }

    private void BeginDrag(Vector2 screenPos)
    {
        // ensures UI clicks won't begin a drag action
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        startWorld = ToWorld(screenPos);
        startCell = board.GetCellAtWorld(startWorld);
        dragging = startCell != null && startCell.tile != null;
    }

    private void UpdateDrag(Vector2 screenPos)
    {
        Vector3 delta = ToWorld(screenPos) - startWorld;
        if (delta.magnitude < swipeThreshold) return;

        // snaps to dominant axis so diagonal swipes resolve to 1 direction only
        Vector2Int dir = Mathf.Abs(delta.x) > Mathf.Abs(delta.y)
            ? new Vector2Int((int)Mathf.Sign(delta.x), 0)
            : new Vector2Int(0, (int)Mathf.Sign(delta.y));

        dragging = false;

        Debug.Log("Finished drag");
        Cell target = board.GetCell(startCell.x + dir.x, startCell.y + dir.y);
        if (target != null && target.active) board.TrySwap(startCell, target);
    }
}
