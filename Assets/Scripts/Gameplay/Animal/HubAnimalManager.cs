using UnityEngine;

/*
 * This script is responsible for spawning all saved animals in the hub level. 
 */

public class HubAnimalManager : MonoBehaviour
{
    [System.Serializable]
    public class AnimalSpawnData
    {
        public AnimalManager.AnimalType animalType;
        public RectTransform spawnPoint;
    }

    [Header("Animal Prefabs")]
    [SerializeField] private GameObject[] animalPrefabs;

    [Header("Animal Spawn Points")]
    [SerializeField] private AnimalSpawnData[] spawnPoints;

    // Spawn all the unlocked animals into hub at start
    private void Start()
    {
        SpawnUnlockedAnimals();

        // index 2 in the music array is for the hub lvl
        AudioManager.Instance.PlayMusic(AudioManager.Instance.bgMusicArray[2]);
    }

    // Search for save data for any animals that have been unlocked
    private void SpawnUnlockedAnimals()
    {
        if (DataPersistenceManager.instance == null)
        {
            return;
        }

        foreach (AnimalSpawnData spawnData in spawnPoints)
        {
            if (spawnData.animalType == AnimalManager.AnimalType.none)
            {
                continue;
            }

            if (spawnData.spawnPoint == null)
            {
                continue;
            }

            if (!DataPersistenceManager.instance.IsAnimalUnlocked(spawnData.animalType))
            {
                continue;
            }

            SpawnAnimal(spawnData);
        }
    }

    // Spawn animal at its designated spawn point, using localScale
    private void SpawnAnimal(AnimalSpawnData spawnData)
    {
        int prefabIndex = (int)spawnData.animalType;

        if (prefabIndex < 0 || prefabIndex >= animalPrefabs.Length)
        {
            return;
        }

        GameObject animal = Instantiate(animalPrefabs[prefabIndex], spawnData.spawnPoint);
        animal.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        animal.GetComponent<RectTransform>().localScale = Vector3.one;

        animal.AddComponent<AnimalTween>();

        Debug.Log($"Spawned {spawnData.animalType}");
    }
}
