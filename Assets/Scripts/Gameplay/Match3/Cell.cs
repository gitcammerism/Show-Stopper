using UnityEngine;

public class Cell 
{
    public int x, y;
    public bool active = true;
    public Vector3 worldPos;
    public Tile tile;
    public Obstacle obstacle;
    public bool CanHoldTile => active && obstacle == null;
}
