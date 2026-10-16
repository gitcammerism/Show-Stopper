using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

// decides when the game ends (either from winning or losing)

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelTimer timer;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private ScoreDisplay scoreDisplay;
    [SerializeField] private BoardInput boardInput;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI gameWinText;
    [SerializeField] private Button restartButton;

    public bool LevelEnded { get; private set; }

    void OnEnable() => timer.TimeUp += OnTimeUp;
    void OnDisable() => timer.TimeUp -= OnTimeUp;

    void Start()
    {
        gameOverText.gameObject.SetActive(false);
        gameWinText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        timer.StartTimer();
    }

    // game over, player lost
    private void OnTimeUp()
    {
        if (LevelEnded) return;
        LevelEnded = true;
        boardInput.inputLocked = true;
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }

    // player won
    public void Win()
    {
        if (LevelEnded) return;
        LevelEnded = true;
        timer.StopTimer();
        boardInput.inputLocked = true;
        StartCoroutine(DrainBonus());
    }

    // drains the timer visibly while score increases based on time left
    private IEnumerator DrainBonus()
    {
        float drainRate = 30f; // seconds of timer converted per real second
        float owed = 0f; // fractional points carried between frames

        while (timer.TimeRemaining > 0f)
        {
            float step = Mathf.Min(drainRate * Time.deltaTime, timer.TimeRemaining);
            timer.SetRemaining(timer.TimeRemaining - step);

            owed += step * scoreManager.pointsPerSecond;
            int whole = Mathf.FloorToInt(owed);
            owed -= whole;
            scoreManager.AddPoints(whole);

            yield return null;
        }
        scoreManager.AddPoints(Mathf.RoundToInt(owed)); // leftover fractional points are added here

        // waits for the displayed score to catch up
        while (!scoreDisplay.IsSettled) yield return null;
        yield return new WaitForSeconds(0.3f); // small pause before continuing


        // logic that goes after drain completes
        gameWinText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }
}
