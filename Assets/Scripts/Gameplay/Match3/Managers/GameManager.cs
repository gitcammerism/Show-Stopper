using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

// decides when the game ends (either from winning or losing)

public class GameManager : MonoBehaviour
{
    public string levelID;

    [SerializeField] private LevelTimer timer;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private ScoreDisplay scoreDisplay;
    [SerializeField] private BoardInput boardInput;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI gameWinText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button restartButtonWin;
    [SerializeField] private GoalTracker goalTracker;
    [SerializeField] private GameObject fireworkVFX;

    [SerializeField] private AnimalManager _animManager;
    private AudioManager _audioManager;
    private SceneSwitcher _sceneSwitcher;
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
        restartButtonWin.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(false);
        settingsPanel.gameObject.SetActive(false);
        gameOverScreen.gameObject.SetActive(false);

        timer.StartTimer();

        // index 3 of music array is level music
        _audioManager = AudioManager.Instance;
        _audioManager.PlayMusic(_audioManager.bgMusicArray[3]);

    
    }

    // game over, player lost
    private void OnTimeUp()
    {
        if (LevelEnded) return;
        LevelEnded = true;
        boardInput.inputLocked = true;
        gameOverText.gameObject.SetActive(true);
        gameOverScreen.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        _audioManager.StopMusic(_audioManager.bgMusicArray[3]);
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

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxArray[2]); // play win sfx
        if (fireworkVFX != null)
        {
            fireworkVFX.GetComponent<ParticleSystem>().Play();
        }

        // FROM COLIN - Save all necessary data
        DataPersistenceManager.instance.RecordLevelScore(levelID, scoreManager.Score);
        DataPersistenceManager.instance.RecordLevelCompletion(levelID);
        DataPersistenceManager.instance.AddCollectedResources(goalTracker.GetCollectedResources());
        _animManager.UnlockAnimal();

        // logic that goes after drain completes
        gameWinText.gameObject.SetActive(true);
        restartButtonWin.gameObject.SetActive(true);
    }

    public void TogglePauseMenu()
    {
        if (!pauseMenu.activeSelf)
        {
            pauseMenu.gameObject.SetActive(true);
            settingsPanel.gameObject.SetActive(false);
            timer.StopTimer();
        }
        else if (pauseMenu.activeSelf)
        {
            pauseMenu.gameObject.SetActive(false);
            settingsPanel.gameObject.SetActive(false);
            // find out if this starts it over
            timer.StartTimer();
        }
        else
        {
            Debug.Log("wtf");
        }
    }

    public void ToggleSettings()
    {
        if (!settingsPanel.activeSelf)
        {
            settingsPanel.gameObject.SetActive(true);
            pauseMenu.gameObject.SetActive(false);
        }
        else if (settingsPanel.activeSelf)
        {
            settingsPanel.gameObject.SetActive(false);
            pauseMenu.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("wtf again");
        }
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }
}
