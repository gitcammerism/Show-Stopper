using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Animal : MonoBehaviour
{
    [Header("Animal")]
    [SerializeField] private AnimalManager.AnimalType animalType;

    // FROM CHANDLER - added public getter variable for AnimalInput.cs
    public AnimalManager.AnimalType AnimalType => animalType;

    [Header("Need Decay")]
    [SerializeField] private float decayInterval = 60f;
    [SerializeField] private int decayAmount = 1;

    [Header("Need Increasing")]
    [SerializeField] private int resourceCost = 1;
    [SerializeField] private int needIncrease = 25;
    [SerializeField] private int affectionIncrease = 5;

    private InputManager inputManager;
    private AnimalData animalData;

    private void OnEnable()
    {
        inputManager = InputManager.instance;

        if (inputManager != null)
        {
            inputManager.OnStartTouch += OnTouch;
        }
    }
    private void OnDisable()
    {
        if (inputManager != null)
        {
            inputManager.OnStartTouch -= OnTouch;
        }

        StopAllCoroutines();
    }

    // Load animal data and start need decay.
    private void Start()
    {
        LoadAnimalData();

        StartCoroutine(DecayNeeds());
    }

    // Load animal individual saved data
    private void LoadAnimalData()
    {
        animalData = DataPersistenceManager.instance.GetAnimalData(animalType);
    }

    // Gradually decrease every need over time
    private IEnumerator DecayNeeds()
    {
        while (true)
        {
            yield return new WaitForSeconds(decayInterval);

            animalData.comfort = Mathf.Max(0, animalData.comfort - decayAmount);
            animalData.food = Mathf.Max(0, animalData.food - decayAmount);
            animalData.water = Mathf.Max(0, animalData.water - decayAmount);
            animalData.treats = Mathf.Max(0, animalData.treats - decayAmount);
            animalData.toys = Mathf.Max(0, animalData.toys - decayAmount);

            SaveAnimalData();
        }
    }

    // Get text showing all needs
    public string GetNeedsText()
    {
        return
            "Comfort: " + animalData.comfort + "\n" +
            "Food: " + animalData.food + "\n" +
            "Water: " + animalData.water + "\n" +
            "Treats: " + animalData.treats + "\n" +
            "Toys: " + animalData.toys + "\n" +
            "Affection: " + animalData.affection + "/100";
    }

    // Increase comfort
    public void ImproveComfort()
    {
        FulfillNeed("Comfort", resourceCost);
    }

    // Increase Food
    public void FeedAnimal()
    {
        FulfillNeed("Food", resourceCost);
    }

    // Increase Water
    public void GiveWater()
    {
        FulfillNeed("Water", resourceCost);
    }

    // Increase Treat
    public void GiveTreat()
    {
        FulfillNeed("Treat", resourceCost);
    }

    // Increase Toy
    public void GiveToy()
    {
        FulfillNeed("Toy", resourceCost);
    }

    // Consume resource and improve need
    private void FulfillNeed(string resourceID, int cost)
    {
        if (animalData == null)
        {
            LoadAnimalData();
        }

        // Cannot improve a need if full
        if (GetNeedValue(resourceID) >= 100)
        {
            Debug.Log(animalType + " already has enough " + resourceID);
            return;
        }

        // Check and consume player resource
        bool resourceConsumed = DataPersistenceManager.instance.SpendResource(resourceID, cost);

        if (!resourceConsumed)
        {
            Debug.Log("Not enough " + resourceID + " resources!");
            return;
        }

        // Increase the appropriate need
        SetNeedValue(resourceID, Mathf.Clamp(GetNeedValue(resourceID) + needIncrease, 0, 100));

        // Increase affection
        animalData.affection = Mathf.Clamp(animalData.affection + affectionIncrease, 0, 100);

        SaveAnimalData();

        Debug.Log(animalType + " gained affection!");
    }

    // Get the current value of a need
    private int GetNeedValue(string resourceID)
    {
        switch (resourceID)
        {
            case "Comfort":
                return animalData.comfort;

            case "Food":
                return animalData.food;

            case "Water":
                return animalData.water;

            case "Treat":
                return animalData.treats;

            case "Toy":
                return animalData.toys;

            default:
                return 0;
        }
    }

    // Change the value of a need
    private void SetNeedValue(string resourceID, int value)
    {
        switch (resourceID)
        {
            case "Comfort":
                animalData.comfort = value;
                break;

            case "Food":
                animalData.food = value;
                break;

            case "Water":
                animalData.water = value;
                break;

            case "Treat":
                animalData.treats = value;
                break;

            case "Toy":
                animalData.toys = value;
                break;
        }
    }

    // Save this animal data
    private void SaveAnimalData()
    {
        DataPersistenceManager.instance.UpdateAnimalData(animalType, animalData);
    }

    private void OnTouch(Vector3 position, float time, InputAction.CallbackContext context)
    {
        // Zoom in on animal here
    }
}
