using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            Debug.Log("Multiple Data Persistence Managers in scene!");
            Destroy(this.gameObject);
            return;
        }

        instance = this;

        // Ensures the DataManager will not be destroyed
        DontDestroyOnLoad(this.gameObject);

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
    }

    // Subscribe to OnSceneLoaded and OnSceneUnloaded
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    // Subscribe to OnSceneLoaded and OnSceneUnloaded
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    // When Scene Loaded, find all files using IDataPersistence
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Loaded Called");
        
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        LoadGame();
    }

    // Save Game whenever player leaves a screen
    public void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("Unloaded Called");

        SaveGame();
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

        // Save data to a file using handler
        dataHandler.Save(gameData);
    }

    // Checks if level is completed then saves
    public void RecordLevelCompletion(string levelID)
    {
        if (gameData == null)
        {
            gameData = dataHandler.Load();

            if (gameData == null)
            {
                gameData = new GameData();
            }
        }

        if (gameData.levelsCompleted == null)
        {
            gameData.levelsCompleted = new SerializableDictionary<string, bool>();
        }

        gameData.levelsCompleted[levelID] = true;

        dataHandler.Save(gameData);
    }

    // Grabs the high score for each level
    public int GetLevelHighScore(string levelID)
    {
        
        // Load game data
        if (gameData == null)
        {
            gameData = dataHandler.Load();

            if (gameData == null)
            {
                gameData = new GameData();
            }
        }

        // Ammends any save files with high score dictionary in case of error
        if (gameData.levelHighScores == null)
        {
            gameData.levelHighScores = new SerializableDictionary<string, int>();
        }

        // Return high score from previous playthrough
        if (gameData.levelHighScores.TryGetValue(levelID, out int highScore))
        {
            return highScore;
        }

        // Return a 0 if the level has no previously saved score
        return 0;
    }

    // Save high score to corresponding level
    public void RecordLevelScore(string levelID, int newScore)
    {
        // Load game data
        if (gameData == null)
        {
            gameData = dataHandler.Load();

            if (gameData == null)
            {
                gameData = new GameData();
            }
        }

        // Ammends any save files with high score dictionary in case of error
        if (gameData.levelHighScores == null)
        {
            gameData.levelHighScores = new SerializableDictionary<string, int>();
        }

        int previousHighScore = GetLevelHighScore(levelID);

        // Update high score if higher than previous
        if (newScore > previousHighScore)
        {
            gameData.levelHighScores[levelID] = newScore;

            dataHandler.Save(gameData);

            Debug.Log($"New high score for {levelID}: {newScore}");
        }
    }

    // Save collected resources from a level
    public void AddCollectedResources(Dictionary<TileType, int> collectedResources)
    {
        // Load game data
        if (gameData == null)
        {
            gameData = dataHandler.Load();

            if (gameData == null)
            {
                gameData = new GameData();
            }
        }

        // Ammends any save files with high score dictionary in case of error
        if (gameData.resources == null)
        {
            gameData.resources = new SerializableDictionary<string, int>();
        }

        // Add resources to players collected resources
        foreach (KeyValuePair<TileType, int> resource in collectedResources)
        {
            string resourceID = resource.Key.ToString();

            // If none of this resource to be collected
            // Continue to next resource
            if (resource.Value <= 0)
            {
                continue;
            }

            // If player has resource already, add more
            // If not, create new entry then add
            if (gameData.resources.ContainsKey(resourceID))
            {
                gameData.resources[resourceID] += resource.Value;
            }
            else
            {
                gameData.resources.Add(resourceID, resource.Value);
            }
        }

        // Save collected resources
        dataHandler.Save(gameData);
    }

    // Load saved resource
    // Return a 0 unless the resource is real and actually has a value tied to it
    public int GetResourceCount(string resourceID)
    {
        if (gameData == null)
        {
            return 0;
        }

        if (gameData.resources == null)
        {
            return 0;
        }

        if (gameData.resources.TryGetValue(resourceID, out int amount))
        {
            return amount;
        }

        return 0;
    }

    // Unlocks animals for saving
    public void UnlockAnimal(AnimalManager.AnimalType animalType)
    {
        if (animalType == AnimalManager.AnimalType.none)
        {
            return;
        }

        if (gameData == null)
        {
            gameData = dataHandler.Load();

            if (gameData == null)
            {
                gameData = new GameData();
            }
        }

        if (gameData.animals == null)
        {
            gameData.animals = new SerializableDictionary<string, AnimalData>();
        }

        string animalID = animalType.ToString();

        if (!gameData.animals.TryGetValue(animalID, out AnimalData animalData))
        {
            animalData = new AnimalData();
            gameData.animals[animalID] = animalData;
        }

        // Unlock the animal in save data
        animalData.isUnlocked = true;

        // Write this to JSON
        dataHandler.Save(gameData);
    }

    // Check to see if animal is unlocked
    public bool IsAnimalUnlocked(AnimalManager.AnimalType animalType)
    {
        if (animalType == AnimalManager.AnimalType.none)
        {
            return false;
        }

        if (gameData == null)
        {
            gameData = dataHandler.Load();

            if (gameData == null)
            {
                gameData = new GameData();
            }
        }

        if (gameData.animals == null)
        {
            return false;
        }

        string animalID = animalType.ToString();

        return gameData.animals.TryGetValue(animalID, out AnimalData animalData) && animalData.isUnlocked;
    }

    // Save the game when quitting the game
    private void OnApplicationQuit()
    {
        SaveGame();
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
