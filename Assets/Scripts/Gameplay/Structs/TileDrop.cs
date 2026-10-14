using UnityEngine;
using Unity.Mathematics;

// This struct stores the information for the tile falling down to fill a board gap.

[System.Serializable]
public struct TileDrop
{
    public Vector3 coordinates;
    public int fromY;

    public TileDrop (Vector3 coordinates, int distance)
    {
        this.coordinates = coordinates;
        fromY = (int)coordinates.y + distance;
    }
}
