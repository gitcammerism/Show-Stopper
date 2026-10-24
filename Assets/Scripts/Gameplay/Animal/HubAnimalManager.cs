using UnityEngine;

public class HubAnimalManager : MonoBehaviour
{
    [System.Serializable]
    public class AnimalSpawnData
    {
        public AnimalManager.AnimalType animalType;
        public Transform spawnPoint;
    }

    [Header("Animal Prefabs")]
    [SerializeField] private GameObject[] animalPrefabs;

    [Header("Animal Spawn Points")]
    [SerializeField] private AnimalSpawnData[] spawnPoints;

    // Spawn all the unlocked animals into hub at start
    private void Start()
    {
        SpawnUnlockedAnimals();
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

    // Spawn animal at its designated spawn point
    private void SpawnAnimal(AnimalSpawnData spawnData)
    {
        int prefabIndex = (int)spawnData.animalType;

        if (prefabIndex < 0 || prefabIndex >= animalPrefabs.Length)
        {
            return;
        }

        GameObject animal = Instantiate(
            animalPrefabs[prefabIndex],
            spawnData.spawnPoint.position,
            spawnData.spawnPoint.rotation
        );

        animal.AddComponent<AnimalTween>();

        Debug.Log($"Spawned {spawnData.animalType}");
    }
}
