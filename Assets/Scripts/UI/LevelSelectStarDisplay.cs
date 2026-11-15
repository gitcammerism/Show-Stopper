using UnityEngine;
using UnityEngine.UI;

public class LevelSelectStarDisplay : MonoBehaviour
{
    [Header("Level Information")]
    [SerializeField] private string levelID;

    [Header("Star Images")]
    [SerializeField] private Image[] stars;

    private void Start()
    {
        UpdateStars();
    }

    // Update displayed stars based on saved data
    public void UpdateStars()
    {
        int savedStars = DataPersistenceManager.instance.GetLevelStars(levelID);

        for (int i = 0; i < stars.Length; i++)
        {
            // Only display earned stars
            stars[i].gameObject.SetActive(i < savedStars);
        }
    }
}
