using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    private GameData gameData;

    public List<IDataPersistence> dataPersistenceObjects;
    
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

    // When started, find all files using IDataPersistence
    private void Start()
    {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
    }

    // Re-intalizes Game Data to start a new game with clear data
    public void NewGame()
    {
        Debug.Log("New Game");

        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        Debug.Log("Loaded Game");

        // COLIN TODO: Load save data from a file

        // If no game data is found, start a new game instead
        if (this.gameData == null)
        {
            Debug.Log("No data was found. Starting a New Game.");
            NewGame();
        }

        // Push loaded data to scripts
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        Debug.Log("Loaded Resources = " + gameData.testResource);
    }

    public void SaveGame()
    {
        Debug.Log("Saved Game");
        
        // Pass data to other scripts to update
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        Debug.Log("Saved Resources = " + gameData.testResource);

        // COLIN TODO: Save data to a file using handler
    }

    // Finds all scripts utilizing IDataPersistence
    // Places them in dataPersistenceObjects
    // Files must be MonoBehaviour
    // Returns a new list
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects
            = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
