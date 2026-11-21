using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalUI : MonoBehaviour
{
    [Header("UI Panel")]
    [SerializeField] private GameObject carePanel;
    [SerializeField] private NeedsHUD needsHUD;

    [Header("Resource Counters")]
    [SerializeField] private TMP_Text comfortResourceCount;
    [SerializeField] private TMP_Text foodResourceCount;
    [SerializeField] private TMP_Text waterResourceCount;
    [SerializeField] private TMP_Text treatResourceCount;
    [SerializeField] private TMP_Text toyResourceCount;

    [Header("Care Buttons")]
    [SerializeField] private Button comfortButton;
    [SerializeField] private Button foodButton;
    [SerializeField] private Button waterButton;
    [SerializeField] private Button treatButton;
    [SerializeField] private Button toyButton;

    [Header("Affection UI")]
    [SerializeField] private Slider affectionBar;
    [SerializeField] private TMP_Text affectionLevelText;

    private Animal selectedAnimal;

    // Activate listeners
    private void Awake()
    {
        comfortButton.onClick.AddListener(FeedComfort);
        foodButton.onClick.AddListener(FeedFood);
        waterButton.onClick.AddListener(FeedWater);
        treatButton.onClick.AddListener(FeedTreat);
        toyButton.onClick.AddListener(FeedToy);

        if (carePanel != null)
        {
            carePanel.SetActive(false);
        }
    }

    // Remove listeners
    private void OnDestroy()
    {
        comfortButton.onClick.RemoveListener(FeedComfort);
        foodButton.onClick.RemoveListener(FeedFood);
        waterButton.onClick.RemoveListener(FeedWater);
        treatButton.onClick.RemoveListener(FeedTreat);
        toyButton.onClick.RemoveListener(FeedToy);
    }

    // Call when animal is selected
    public void Show(Animal animal)
    {
        SetSelectedAnimal(animal);

        if (carePanel != null)
        {
            carePanel.SetActive(true);
        }

        Refresh();
    }

    // Update counters and needs
    public void Refresh()
    {
        Debug.Log("Refreshed!");

        if (selectedAnimal == null || DataPersistenceManager.instance == null)
        {
            return;
        }

        DataPersistenceManager data = DataPersistenceManager.instance;

        comfortResourceCount.text = data.GetResourceCount("Comfort").ToString();
        foodResourceCount.text = data.GetResourceCount("Food").ToString();
        waterResourceCount.text = data.GetResourceCount("Water").ToString();
        treatResourceCount.text = data.GetResourceCount("Treat").ToString();
        toyResourceCount.text = data.GetResourceCount("Toy").ToString();

        comfortButton.interactable = data.GetResourceCount("Comfort") > 0 && selectedAnimal.GetNeedValue("Comfort") < 100;
        foodButton.interactable = data.GetResourceCount("Food") > 0 && selectedAnimal.GetNeedValue("Food") < 100;
        waterButton.interactable = data.GetResourceCount("Water") > 0 && selectedAnimal.GetNeedValue("Water") < 100;
        treatButton.interactable = data.GetResourceCount("Treat") > 0 && selectedAnimal.GetNeedValue("Treat") < 100;
        toyButton.interactable = data.GetResourceCount("Toy") > 0 && selectedAnimal.GetNeedValue("Toy") < 100;

        // Update affection bar and level counter
        if (affectionBar != null)
        {
            affectionBar.maxValue = 100;
            affectionBar.value = selectedAnimal.GetAffection();
        }

        if (affectionLevelText != null)
        {
            affectionLevelText.text = "" + selectedAnimal.GetAffectionLevel();
        }

        if (needsHUD != null)
        {
            needsHUD.RefreshRings();
        }
    }

    private void FeedComfort() => selectedAnimal?.ImproveComfort();
    private void FeedFood() => selectedAnimal?.FeedAnimal();
    private void FeedWater() => selectedAnimal?.GiveWater();
    private void FeedTreat() => selectedAnimal?.GiveTreat();
    private void FeedToy() => selectedAnimal?.GiveToy();

    // Set the animal selected for Needs HUD
    public void SetSelectedAnimal(Animal animal)
    {
        selectedAnimal = animal;

        if (needsHUD != null)
        {
            needsHUD.SetAnimal(animal.AnimalType);
        }
    }
}