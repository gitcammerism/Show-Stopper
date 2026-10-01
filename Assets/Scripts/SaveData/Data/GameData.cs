using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // For determining levels completed
    public SerializableDictionary<string, bool> levelsCompleted;

    // Defines default values for game data
    public GameData()
    {
        levelsCompleted = new SerializableDictionary<string, bool>();
    }
}
