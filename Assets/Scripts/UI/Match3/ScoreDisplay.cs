using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private float countUpSpeed = 400f; // points per second

    // so that the game waits until the timer finishes draining and the score catches up
    public bool IsSettled => Mathf.RoundToInt(shown) == scoreManager.Score;
    private float shown;

    void Start()
    {
        int highScore = DataPersistenceManager.instance.GetLevelHighScore(gameManager.levelID);
        highScoreText.text = "Highscore: " + highScore.ToString();

        shown = scoreManager.Score;
    }

    void Update()
    {
        shown = Mathf.MoveTowards(shown, scoreManager.Score, countUpSpeed * Time.deltaTime);
        scoreText.text = Mathf.RoundToInt(shown).ToString();
    }
}
