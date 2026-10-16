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

    // Each saved resource gained from levels
    public SerializableDictionary<string, int> resources;

    // All information of animals
    public SerializableDictionary<string, AnimalData> animals;

    // Defines default values for game data
    public GameData()
    {
        levelsCompleted = new SerializableDictionary<string, bool>();
        levelHighScores = new SerializableDictionary<string, int>();
        resources = new SerializableDictionary<string, int>();
        animals = new SerializableDictionary<string, AnimalData>();
    }
}
