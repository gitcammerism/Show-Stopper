using System;
using UnityEngine;

[Serializable]
public class AnimalData
{
    // If player unlocked animal
    public bool isUnlocked;

    // Affection level of animal
    public int affection;

    // Every animal stat
    public int comfort;
    public int food;
    public int water;
    public int treats;
    public int toys;

    // Name of the animal
    public string animalName;

    // For the JSON serializer
    public AnimalData()
    {
        isUnlocked = false;

        affection = 0;

        comfort = 0;
        food = 0;
        water = 0;
        treats = 0;
        toys = 0;

        animalName = "";
    }
}