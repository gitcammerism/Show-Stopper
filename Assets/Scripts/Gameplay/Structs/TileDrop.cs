using Unity.Mathematics;

/*
 * Last Modified: 09/23/2026 by Chandler Guzman
 * 
 * This struct stores the information for the tile falling down to fill a board gap.
 */

[System.Serializable]
public struct TileDrop
{
    public int2 coordinates;
    public int fromY;

    public TileDrop (int x, int y, int distance)
    {
        coordinates.x = x;
        coordinates.y = y;
        fromY = y + distance;
    }
}
