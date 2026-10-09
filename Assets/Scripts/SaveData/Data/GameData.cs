using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // For determining levels completed
    public SerializableDictionary<string, bool> levelsCompleted;

    // For showing the highest score of each level
    public SerializableDictionary<string, int> levelHighScores;

    // Defines default values for game data
    public GameData()
    {
        levelsCompleted = new SerializableDictionary<string, bool>();
        levelHighScores = new SerializableDictionary<string, int>();
    }
}
