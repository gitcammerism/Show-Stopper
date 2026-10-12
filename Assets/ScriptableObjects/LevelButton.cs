using UnityEngine;
using UnityEngine.UI;

// This script is responsible for creating a new level select button. 

public class LevelButton : MonoBehaviour
{
    private int _levelNum;
    private LevelButton _lvlButton;

    // public getter for levelNum
    public int LevelNum => _levelNum;
    public LevelButton LevelBtn => _lvlButton;

    // public constructor that creates a new level button
    public LevelButton(int levelNum)
    {
        _levelNum = levelNum;
        _lvlButton = this;
    }

    public LevelButton SpawnButton()
    {
        return _lvlButton;
    }
}
