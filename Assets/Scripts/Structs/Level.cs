using UnityEngine;
using UnityEngine.UI;

// DEPRACATED: not using

[System.Serializable]
public struct Level
{
    private int _levelNum;
    private LevelButton _lvlButton;

    // public getter for levelNum
    public int LevelNum => _levelNum;
    public LevelButton LevelBtn => _lvlButton;

    // public constructor that creates a new level button
    public Level(int levelNum)
    {
        _levelNum = levelNum;
        _lvlButton = null;
    }
}
