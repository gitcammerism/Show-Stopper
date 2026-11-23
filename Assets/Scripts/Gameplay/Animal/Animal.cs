using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/**
 * This script is responsible for loading animal data, managing player input, and managing the needs of an individual animal in the hub.
 */

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

    [Header("VFX")]
    [SerializeField] private int lowNeedThreshold = 50;
    private AnimalVFX _animalVFX;

    private AnimalInput _animalInput;
    private AnimalData animalData;
    private Button _animalButton;
    private AnimalUI careUI;
    private bool inFocus = false;

    private void Awake()
    {
        _animalButton = GetComponent<Button>();
        _animalButton.onClick.AddListener(OnTouch);

        _animalInput = FindFirstObjectByType<AnimalInput>();

        _animalVFX = FindFirstObjectByType<AnimalVFX>();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    // Load animal data and start need decay.
    private void Start()
    {
        LoadAnimalData();
        StartCoroutine(DecayNeeds());
    }

    public void Initialize(AnimalUI sharedUI)
    {
        careUI = sharedUI;
        LoadAnimalData();
    }

    // Load animal individual saved data
    private void LoadAnimalData()
    {
        animalData = DataPersistenceManager.instance.GetAnimalData(animalType);

        if (animalData.affectionLevel < 0)
        {
            animalData.affectionLevel = 0;
            SaveAnimalData();
        }
    }

    // Gradually decrease every need over time
    private IEnumerator DecayNeeds()
    {
        while (true)
        {
            if (inFocus)
            {
                RefreshVFX();
            }
            
            yield return new WaitForSeconds(decayInterval);

            animalData.comfort = Mathf.Max(0, animalData.comfort - decayAmount);
            animalData.food = Mathf.Max(0, animalData.food - decayAmount);
            animalData.water = Mathf.Max(0, animalData.water - decayAmount);
            animalData.treats = Mathf.Max(0, animalData.treats - decayAmount);
            animalData.toys = Mathf.Max(0, animalData.toys - decayAmount);

            SaveAnimalData();
            careUI?.Refresh();

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
        AddAffection(affectionIncrease);

        SaveAnimalData();
        careUI?.Refresh();

        if (inFocus)
        {
            RefreshVFX();
        }

        Debug.Log(animalType + " gained affection!");
    }

    // Get the current value of a need
    public int GetNeedValue(string resourceID)
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

    // Affection methods
    public int GetAffection()
    {
        return animalData != null ? animalData.affection : 0;
    }

    public int GetAffectionLevel()
    {
        return animalData != null ? animalData.affectionLevel : 0;
    }

    // Level up affection when 100 is reached
    private void AddAffection(int amount)
    {
        animalData.affection += amount;

        while (animalData.affection >= 100)
        {
            animalData.affection -= 100;
            animalData.affectionLevel++;

            Debug.Log(animalType + " reached affection level " + animalData.affectionLevel);
        }
    }

    // Save this animal data
    private void SaveAnimalData()
    {
        DataPersistenceManager.instance.UpdateAnimalData(animalType, animalData);
    }

    // public setter for inFocus state
    public void SetFocusState(bool state)
    {
        inFocus = state;

        if(inFocus)
        {
            RefreshVFX();
        }
        else
        {
            _animalVFX.HideAll();
        }
    }

    private void OnTouch()
    {
        // zoom on animal, stop the jumping around and show the needs UI

        if (!inFocus && _animalInput != null)
        {
            SetFocusState(true);

            if (careUI != null)
            {
                careUI.Show(this);
            }

            _animalInput.FocusOnAnimal(this);
        }

    }

    // Check if any need is below the lowNeedThreshold
    private bool HasLowNeed()
    {
        return animalData.comfort <= lowNeedThreshold ||
               animalData.food <= lowNeedThreshold ||
               animalData.water <= lowNeedThreshold ||
               animalData.treats <= lowNeedThreshold ||
               animalData.toys <= lowNeedThreshold;
    }

    // Refresh the VFX based on the animal's current needs
    private void RefreshVFX()
    {
        if (animalData != null && _animalVFX != null)
        {
            _animalVFX.SetMood(HasLowNeed());
        }
    }
}
