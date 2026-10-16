using UnityEngine;
using UnityEngine.UI;

public class GoalDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GoalTracker tracker;
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Image backgroundImage;

    [Header("Customization")]
    [SerializeField] private int goalIndex;
    [SerializeField] private float fillSpeed = 1.5f;
    private float targetValue;

    void Start()
    {
        if (goalIndex >= tracker.GoalCount)
        {
            // change slider to grayed out background color
            gameObject.SetActive(false); return;
        }

        // change slider fill and background colors based on goal
        fillImage.color = tracker.fillColors[goalIndex];
        backgroundImage.color = tracker.backgroundColors[goalIndex];

        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;

        tracker.Changed += Refresh;
        Refresh();
    }

    void Update()
    {
        slider.value = Mathf.MoveTowards(slider.value, targetValue, fillSpeed * Time.deltaTime);
    }

    void OnDestroy() => tracker.Changed -= Refresh;

    private void Refresh()
    {
        var goal = tracker.GetGoal(goalIndex);
        targetValue = (float)tracker.GetCollected(goalIndex) / Mathf.Max(1, goal.target);
    }
}
