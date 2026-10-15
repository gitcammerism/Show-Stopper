using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelTimer timer;
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

    private void OnTimeUp()
    {
        if (LevelEnded) return;
        LevelEnded = true;
        boardInput.inputLocked = true;
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }

    public void Win()
    {
        if (LevelEnded) return;
        LevelEnded = true;
        timer.StopTimer();
        boardInput.inputLocked = true;
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
