using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    private GameData gameData;
    
    // Instance can be referenced publicy
    // Instance can only be modified in this class
    public static DataPersistenceManager instance { get; private set; }

    // Makes sure there are no other Managers on objects in the scene
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Multiple Data Persistence Managers in scene!");
        }

        instance = this;
    }

    // Re-intalizes Game Data to start a new game with clear data
    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // COLIN TODO: Load save data from a file
        
        // If no game data is found, start a new game instead
        if (this.gameData == null)
        {
            Debug.Log("No data was found. Starting a New Game.");
            NewGame();
        }

        // COLIN TODO: Push loaded data to scripts
    }

    public void SaveGame()
    {
        // COLIN TODO: Pass data to other scripts to update

        // COLIN TODO: Save data to a file using handler
    }
}
