using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

// This script tracks the game state and handles the logic for the match-3 game.

public class Match3Game : MonoBehaviour
{
    public void ClearGrid()
    {
        ClearedTiles.Clear();
        DroppedTiles.Clear();
    }
}
