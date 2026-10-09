using System;
using UnityEngine;
using UnityEngine.UI;

// This script handles the game timer above the match grid.

public class TimeManager : MonoBehaviour
{
    [SerializeField] private Match3Skin gameSkin;
    [SerializeField] private Slider timerSlider;
    
    [Tooltip("Please enter the time limit of the level in seconds.")]
    [SerializeField] private float gameTime;

    public static event Action<bool> OnTimerEnd;

    // Sets timer values & subscribes to stop timer event.
    private void OnEnable()
    {
        Match3GameController.StopTimer += StopTimer;
        timerSlider.maxValue = gameTime;
        timerSlider.value = gameTime;
    }

    // Unsubscribes to stop timer event.
    void OnDisable()
    {
        Match3GameController.StopTimer -= StopTimer;
    }

    // Ticks down the timer every second until 0 is reached.
    void Update()
    {
        float time = gameTime - Time.timeSinceLevelLoad;
        if (time <= 0)
        {
            OnTimerEnd?.Invoke(false);
            timerSlider.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
        else timerSlider.value = time;
    }

    // Stops the timer before 0 is reached.
    private void StopTimer()
    {
        timerSlider.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
