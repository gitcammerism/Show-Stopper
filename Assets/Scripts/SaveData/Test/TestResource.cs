using TMPro;
using UnityEngine;
using UnityEngine.VFX;

// Temporary script just for save testing
public class TestResource : MonoBehaviour, IDataPersistence
{
    public int testResource;
    public int audioTest;
    public bool levelCompleted = false;

    // For UI Updates
    public TextMeshProUGUI resourceText;
    public TextMeshProUGUI audioText;
    public TextMeshProUGUI levelText;

    // For generating unique level ids
    [SerializeField] private string ID;

    [ContextMenu("Generate Guid for ID")]
    private void GenerateGuid()
    {
        ID = System.Guid.NewGuid().ToString();
    }


    void Update()
    {
        UpdateResourceText();
        UpdateAudioSlider();
        UpdateLevelText();
    }

    // Taken from IDataPersistence
    // LoadData loads saved data about the test resource
    // SaveData saves the data about the test resource
    // Also sees if the level has been completed
    public void LoadData(GameData data)
    {
        this.testResource = data.testResource;

        data.levelsCompleted.TryGetValue(ID, out levelCompleted);
    }

    public void SaveData(ref GameData data)
    {
        data.testResource = this.testResource;

        if (data.levelsCompleted.ContainsKey(ID))
        {
            data.levelsCompleted.Remove(ID);
        }

        data.levelsCompleted.Add(ID, levelCompleted);
    }

    public void UpdateResourceText()
    {
        resourceText.text = "Resource Tracker: " + testResource.ToString();
    }

    // For Slider and PlayerPref testing
    public void UpdateAudioSliderText()
    {
        audioText.text = "Audio Level: " + audioTest.ToString() + "%";
    }

    public void UpdateAudioSlider()
    {

    }

    // Add and removing resource test functions
    public void AddTestResource()
    {
        testResource += 1;
        Debug.Log("Resource Added!");
    }
    
    public void SubtractTestResource()
    {
        testResource -= 1;
        Debug.Log("Resource Removed!");
    }

    // Switches level bool to true or false, to test level completion saving
    public void CompleteLevel()
    {
        if (levelCompleted == false)
        {
            levelCompleted = true;
        } else if (levelCompleted == true)
        {
            levelCompleted = false;
        }
    }

    public void UpdateLevelText()
    {
        levelText.text = "Level Complete: " + levelCompleted.ToString();
    }
}
