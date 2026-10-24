using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : MonoBehaviour, IDataPersistence
{
    // This script handles the animal spawning and scaling in the levels
    public enum AnimalType
    {
        none = -1,
        Elephant,
        Giraffe,
        Horse,
        Lion,
        Parrot,
        Rabbit,
        Seal,
    }

    [SerializeField] RectTransform spawnPosition;
    [SerializeField] private float animalScaleMultiplier;

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
            case AnimalType.Elephant:
                InstantiateAnimal();
                Debug.Log("This is an elephant.");
                break;
            case AnimalType.Giraffe:
                InstantiateAnimal();
                Debug.Log("This is a giraffe.");
                break;
            case AnimalType.Horse:
                InstantiateAnimal();
                Debug.Log("This is a horse.");
                break;
            case AnimalType.Lion:
                InstantiateAnimal();
                Debug.Log("This is a lion.");
                break;
            case AnimalType.Parrot:
                InstantiateAnimal();
                Debug.Log("This is a parrot.");
                break;
            case AnimalType.Rabbit:
                InstantiateAnimal();
                Debug.Log("This is a rabbit.");
                break;
            case AnimalType.Seal:
                InstantiateAnimal();
                Debug.Log("This is a seal.");
                break;
            default:
                Debug.Log("Unknown animal type.");
                break;
        }
        if (currentAnimal != null)
        {
            currentAnimal.transform.DOScale(currentAnimal.transform.localScale*1.1f, 1f).SetEase(Ease.OutBounce).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            Debug.Log("animal not created");
        }
    }

    // Cami - Instantiates the animal, sets the position, and scales down the size by the set multiplier
    private void InstantiateAnimal()
    {
        // Scales animal down from native size in order to fit in the cage
        animalScaleMultiplier = 0.08f;

        currentAnimal = Instantiate(animalPrefab[(int)animalType], spawnPosition);
        currentAnimal.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        Vector3 nativeScale = currentAnimal.GetComponent<RectTransform>().localScale;
        Vector3 targetScale = Vector3.Scale(nativeScale, new Vector3(animalScaleMultiplier, animalScaleMultiplier, 1f));
        currentAnimal.GetComponent<RectTransform>().localScale = targetScale;
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

        if (DataPersistenceManager.instance != null)
        {
            DataPersistenceManager.instance.UnlockAnimal(animalType);
        }

        Debug.Log($"{animalType} has been unlocked");
    }
}
