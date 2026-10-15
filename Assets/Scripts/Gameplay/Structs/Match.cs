using UnityEngine;
using Unity.Mathematics;

// This struct stores the values of a given match for move validation.

[System.Serializable]
public struct Match
{
    public Vector3 location;
    public int length;
    public bool isHorizontal;
    public TileType tile;

    public Match (Vector3 position, int length, bool isHorizontal, TileType tile)
    {
        this.location = position;
        this.length = length;
        this.isHorizontal = isHorizontal;
        this.tile = tile;
    }
}
