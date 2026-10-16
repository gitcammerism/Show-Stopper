using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private float countUpSpeed = 400f; // points per second

    private float shown;

    void Start() => shown = scoreManager.Score;

    void Update()
    {
        shown = Mathf.MoveTowards(shown, scoreManager.Score, countUpSpeed * Time.deltaTime);
        scoreText.text = Mathf.RoundToInt(shown).ToString();
    }
}
