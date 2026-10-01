using UnityEngine;
using UnityEngine.UI;

// This script handles the game timer above the match grid.

public class TimeManager : MonoBehaviour
{
    [SerializeField] private Match3Skin gameSkin;
    [SerializeField] private Slider timerSlider;
    [SerializeField] private float gameTime;
    private bool _stopTimer;

    void Start()
    {
        _stopTimer = false;
        timerSlider.maxValue = gameTime;
        timerSlider.value = gameTime;
    }

    // Update is called once per frame
    void Update()
    {
        float time = gameTime - Time.time;
        if (time <= 0)
        {
            _stopTimer = true;
            if (!gameSkin.IsBusy) gameSkin.gameOver = true;
            timerSlider.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
        }

        if (_stopTimer == false)
        {
            timerSlider.value = time;
        }
    }
}
