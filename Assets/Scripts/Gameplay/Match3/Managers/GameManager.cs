using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

// decides when the game ends (either from winning or losing)

public class GameManager : MonoBehaviour
{
    public string levelID;

    [SerializeField] private LevelTimer timer;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private ScoreDisplay scoreDisplay;
    [SerializeField] private BoardInput boardInput;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI gameWinText;
    [SerializeField] private Button restartButton;
    [SerializeField] private GoalTracker goalTracker;
    [SerializeField] private ParticleSystem fireworkVFX;

    // FROM COLIN - For game saving
    [SerializeField] private AnimalManager.AnimalType rewardAnimal = AnimalManager.AnimalType.none;

    public bool LevelEnded { get; private set; }

    // FROM COLIN - Give each level a unique GUID
    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (gameObject == null)
        {
            return;
        }

        if (!gameObject.scene.IsValid())
        {
            return;
        }

        string sceneName = gameObject.scene.name;

        if (sceneName.StartsWith("Level"))
        {
            string levelNumber = sceneName.Substring(5);

            if (int.TryParse(levelNumber, out int levelNum))
            {
                levelID = levelNum.ToString();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
    }
    #endif

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
        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxArray[1]); // play lose sfx
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

        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxArray[2]); // play win sfx
        fireworkVFX.Play();

        // FROM COLIN - Save all necessary data
        DataPersistenceManager.instance.RecordLevelScore(levelID, scoreManager.Score);
        DataPersistenceManager.instance.RecordLevelCompletion(levelID);
        DataPersistenceManager.instance.AddCollectedResources(goalTracker.GetCollectedResources());
        DataPersistenceManager.instance.UnlockAnimal(rewardAnimal);

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
