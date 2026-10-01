using Unity.Mathematics;

/*
 * Last Modified: 09/21/2026 by Chandler Guzman
 * 
 * This struct stores the values of a given match for move validation.
 */

[System.Serializable]
public struct Match
{
    public int2 coordinates;
    public int length;
    public bool isHorizontal;
    public TileState tile;

    public Match (int x, int y, int length, bool isHorizontal, TileState tile)
    {
        coordinates.x = x;
        coordinates.y = y;
        this.length = length;
        this.isHorizontal = isHorizontal;
        this.tile = tile;
    }
}
