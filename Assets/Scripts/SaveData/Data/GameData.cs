using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // A temporary int just for testing the save system
    public int testResource;

    // For determining levels completed
    public SerializableDictionary<string, bool> levelsCompleted;

    // Defines default values for game data
    public GameData()
    {
        this.testResource = 0;
        levelsCompleted = new SerializableDictionary<string, bool>();
    }
}
