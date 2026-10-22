using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : MonoBehaviour, IDataPersistence
{
    // This script handles the animal spawning and scaling in the levels
    public enum AnimalType
    {
        none = -1,
        Horse,
        Lion,
        Parrot,
        Rabbit,
        Seal,
    }

    [SerializeField] Transform spawnPosition;

    public GameObject[] animalPrefab;

    public GameObject currentAnimal;

    public AnimalType animalType;

    // FROM COLIN - For saving animals
    [SerializeField]
    private bool isUnlocked;
    public bool IsUnlocked => isUnlocked;

    private void Start()
    {
        if (spawnPosition) {             SpawnAnimal();
        }
        else
        {
            Debug.LogError("Spawn position is not assigned.");
        }
    }

    private void SpawnAnimal()
    {
        switch (animalType)
        {
            case AnimalType.none:
                Debug.Log("No animal selected.");
                break;
            case AnimalType.Horse:
                currentAnimal = Instantiate(animalPrefab[(int)animalType], spawnPosition.position, Quaternion.identity);
                Debug.Log("This is a horse.");
                break;
            case AnimalType.Lion:
                currentAnimal = Instantiate(animalPrefab[(int)animalType], spawnPosition.position, Quaternion.identity);
                Debug.Log("This is a lion.");
                break;
            case AnimalType.Parrot:
                Debug.Log("This is a parrot.");
                currentAnimal = Instantiate(animalPrefab[(int)animalType], spawnPosition.position, Quaternion.identity);
                break;
            case AnimalType.Rabbit:
                Debug.Log("This is a rabbit.");
                currentAnimal = Instantiate(animalPrefab[(int)animalType], spawnPosition.position, Quaternion.identity);
                break;
            case AnimalType.Seal:
                Debug.Log("This is a seal.");
                currentAnimal = Instantiate(animalPrefab[(int)animalType], spawnPosition.position, Quaternion.identity);
                break;
            default:
                Debug.Log("Unknown animal type.");
                break;
        }
        if (currentAnimal != null)
        {
            currentAnimal.transform.DOScale(currentAnimal.transform.lossyScale*1.1f, 1f).SetEase(Ease.OutBounce).SetLoops(-1, LoopType.Yoyo);
        }
    }

    // FROM COLIN - Force animal to be unlocked
    public void SetUnlocked()
    {
        isUnlocked = true;
    }

    // FROM COLIN - Called when scene is loaded
    public void LoadData(GameData data)
    {
        if (data.animals == null)
        {
            data.animals = new SerializableDictionary<string, AnimalData>();
        }

        string animalID = animalType.ToString();

        if (data.animals.TryGetValue(animalID, out AnimalData savedAnimal))
        {
            isUnlocked = savedAnimal.isUnlocked;
        }
        else
        {
            // Animal stays locked until unlocked
            isUnlocked = false;
        }

        Debug.Log($"{animalID} unlocked = {isUnlocked}");
    }

    // FROM COLIN - Called when scene is saved (Exited/Level Completed)
    public void SaveData(ref GameData data)
    {
        if (animalType == AnimalType.none)
        {
            return;
        }

        if (data.animals == null)
        {
            data.animals = new SerializableDictionary<string, AnimalData>();
        }

        string animalID = animalType.ToString();

        // Keep an unlock that has been saved as such
        bool alreadyUnlocked = data.animals.TryGetValue(animalID, out AnimalData savedAnimal)
            && savedAnimal != null
            && savedAnimal.isUnlocked;

        if (!data.animals.ContainsKey(animalID) || data.animals[animalID] == null)
        {
            data.animals[animalID] = new AnimalData();
        }

        // Unlocks remain true
        data.animals[animalID].isUnlocked = isUnlocked || alreadyUnlocked;
    }

    // FROM COLIN - Call when animal should be unlocked
    public void UnlockAnimal()
    {
        if (isUnlocked)
        {
            return;
        }

        isUnlocked = true;

        Debug.Log($"{animalType} has been unlocked");

        if (DataPersistenceManager.instance != null)
        {
            DataPersistenceManager.instance.SaveGame();
        }
    }
}
