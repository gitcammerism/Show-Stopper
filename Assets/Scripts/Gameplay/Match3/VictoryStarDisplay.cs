using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VictoryStarDisplay : MonoBehaviour
{
    [Header("Star Images")]
    [SerializeField] private Image[] stars;

    [Header("Animation")]
    [SerializeField] private float popDuration = 0.35f;
    [SerializeField] private float popDelay = 0.2f;
    [SerializeField] private float popScale = 1.4f;

    private Coroutine animationCoroutine;

    // Hide all stars until level is completed
    private void Awake()
    {
        foreach (Image star in stars)
        {
            star.gameObject.SetActive(false);
        }
    }

    // Show stars if earned at end of level
    public void ShowStars(int earnedStars)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].gameObject.SetActive(i < earnedStars);
        }
    }
}