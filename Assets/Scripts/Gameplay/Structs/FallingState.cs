using UnityEngine;

// This serializable struct stores the current state of a given falling tile.

[System.Serializable]
public struct FallingState
{
    public float fromY, toY, duration, progress;
}
