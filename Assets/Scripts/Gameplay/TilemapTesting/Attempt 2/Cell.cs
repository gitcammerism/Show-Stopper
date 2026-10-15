using UnityEngine;

public class Cell : MonoBehaviour
{
    public int x, y;
    public bool active;
    public Vector3 worldPos;
    public Tile tile;
    public Obstacle obstacle;
    public bool CanHoldTile => active && obstacle == null;
}
