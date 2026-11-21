using System;
using UnityEngine;

[Serializable]
public class AnimalData
{
    // If player unlocked animal
    public bool isUnlocked;

    // Affection level of animal
    public int affection;
    public int affectionLevel;

    // Every animal stat
    public int comfort;
    public int food;
    public int water;
    public int treats;
    public int toys;

    // Name of the animal
    public string animalName;

    // FROM CHANDLER - randomizer for starting needs
    private static readonly System.Random randomizer = new();

    // For the JSON serializer
    public AnimalData()
    {
        isUnlocked = false;

        affection = 0;
        affectionLevel = 0;

        comfort = randomizer.Next(50, 101);
        food = randomizer.Next(50, 101);
        water = randomizer.Next(50, 101);
        treats = randomizer.Next(50, 101);
        toys = randomizer.Next(50, 101);

        animalName = "";
    }
}