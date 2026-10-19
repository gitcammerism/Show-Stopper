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

            // change slider to grayed out background color (will make this a variable later bleh)
            Color color = new();
            if (ColorUtility.TryParseHtmlString("#ABA7A7", out color))
            {
                Debug.Log("aaaa");
                backgroundImage.color = color;
            }
            return;
        }

        // change slider fill and background colors based on goal
        fillImage.sprite = tracker.fillImages[(int)tracker.GetGoal(goalIndex).tileType];
        backgroundImage.sprite = tracker.backgroundImages[(int)tracker.GetGoal(goalIndex).tileType];

        fillImage.color = Color.white;

        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;

        tracker.Changed += Refresh;
        Refresh();
    }

    void Update()
    {
        if (goalIndex >= tracker.GoalCount) return;
        slider.value = Mathf.MoveTowards(slider.value, targetValue, fillSpeed * Time.deltaTime);
    }

    void OnDestroy() => tracker.Changed -= Refresh;

    // updates slider value
    private void Refresh()
    {
        var goal = tracker.GetGoal(goalIndex);
        targetValue = (float)tracker.GetCollected(goalIndex) / Mathf.Max(1, goal.target);
    }
}
