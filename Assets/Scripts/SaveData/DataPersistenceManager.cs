using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;
    
    private GameData gameData;

    public List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;
    
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
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        LoadGame();
    }

    // Delete Game Data
    public void DeleteData()
    {
        dataHandler.Delete();

        // Load game to match deleted data
        LoadGame();
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

        // Load save data from a file
        this.gameData = dataHandler.Load();

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

        if (this.gameData == null)
        {
            Debug.Log("No data was found. Starting a New Game.");
            NewGame();
        }

        // Pass data to other scripts to update
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        Debug.Log("Saved Resources = " + gameData.testResource);

        // Save data to a file using handler
        dataHandler.Save(gameData);
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

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
