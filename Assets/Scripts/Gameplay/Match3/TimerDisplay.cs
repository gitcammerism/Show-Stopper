using UnityEngine;
using UnityEngine.UI;

// Handles the updating of the timer Slider.

public class TimerDisplay : MonoBehaviour
{
    [SerializeField] private LevelTimer timer;
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient colorOverTime; // used for urgency - base color at 1, urgent color at 0

    void Start()
    {
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
    }

    void Update()
    {
        float t = timer.TimeRemaining / timer.Duration;
        slider.value = t;
        fillImage.color = colorOverTime.Evaluate(t);
    }
}
