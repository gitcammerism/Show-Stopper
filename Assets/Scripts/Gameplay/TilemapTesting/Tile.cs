using UnityEngine;

// Tile class which stores all the information about a given tile.

public class Tile : MonoBehaviour
{
    [System.NonSerialized] 
    public TileState tileState;
    public TileType tileType;
}
