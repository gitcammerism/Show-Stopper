using TMPro;
using UnityEngine;

// Temporary script just for save testing
public class TestResource : MonoBehaviour
{
    public int testResource;
    public int audioTest;

    // For UI Updates
    public TextMeshProUGUI resourceText;
    public TextMeshProUGUI audioText;

    void Update()
    {
        UpdateResourceText();
        UpdateAudioSlider();
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
}
