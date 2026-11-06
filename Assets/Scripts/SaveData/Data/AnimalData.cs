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

        comfort = 100;
        food = 100;
        water = 100;
        treats = 100;
        toys = 100;

        animalName = "";
    }
}