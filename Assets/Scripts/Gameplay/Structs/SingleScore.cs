using UnityEngine;
using Unity.Mathematics;

// Serializable struct that stores the info for the floating score above a match that has been made.

[System.Serializable]
public struct SingleScore
{
    public Vector3 position;
    public int value;
}
