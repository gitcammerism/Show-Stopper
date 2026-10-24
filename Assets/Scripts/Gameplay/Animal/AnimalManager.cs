using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : MonoBehaviour
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

    [SerializeField] RectTransform spawnPosition;

    public GameObject[] animalPrefab;

    public GameObject currentAnimal;

    public AnimalType animalType;

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
                currentAnimal = Instantiate(animalPrefab[(int)animalType], spawnPosition);
                currentAnimal.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                currentAnimal.GetComponent<RectTransform>().localScale = Vector3.one;
                Debug.Log("This is a horse.");
                break;
            case AnimalType.Lion:
                currentAnimal = Instantiate(animalPrefab[(int)animalType], spawnPosition);
                currentAnimal.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                currentAnimal.GetComponent<RectTransform>().localScale = Vector3.one; Debug.Log("This is a lion.");
                break;
            case AnimalType.Parrot:
                Debug.Log("This is a parrot.");
                currentAnimal = Instantiate(animalPrefab[(int)animalType], spawnPosition);
                currentAnimal.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                currentAnimal.GetComponent<RectTransform>().localScale = Vector3.one; 
                break;
            case AnimalType.Rabbit:
                Debug.Log("This is a rabbit.");
                currentAnimal = Instantiate(animalPrefab[(int)animalType], spawnPosition);
                currentAnimal.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                currentAnimal.GetComponent<RectTransform>().localScale = Vector3.one; 
                break;
            case AnimalType.Seal:
                Debug.Log("This is a seal.");
                currentAnimal = Instantiate(animalPrefab[(int)animalType], spawnPosition);
                currentAnimal.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                currentAnimal.GetComponent<RectTransform>().localScale = Vector3.one; 
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
}
