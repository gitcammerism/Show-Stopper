using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * This script is responsible for loading levels when the corresponding button is pressed. 
 * This script may also be expanded to handle buttons being unavailable for unlocked levels.
 */

public class LevelButton : MonoBehaviour
{
    private int _levelNum;
    private Button _lvlButton;
    [SerializeField] private SceneSwitcher _sceneSwitcher;
    private LevelSelect _levelSelect;

    private void Awake()
    {
        // Get the button component from the child object and grab the SceneSwitcher and LevelSelectManager
        _lvlButton = this.GetComponentInChildren<Button>();
        _sceneSwitcher = FindAnyObjectByType<SceneSwitcher>();
        _levelSelect = FindAnyObjectByType<LevelSelect>();
        _lvlButton.onClick.AddListener(OnPress);
    }

    private void OnEnable()
    {
        if (_lvlButton != null && _levelSelect != null)
        {
            Invoke(nameof(GreyOut), 0.01f);
        }
    }

    // check if this is needed
    private void Start()
    {
        Invoke(nameof(GreyOut), 0.01f);
    }

    // Called when the button is pressed. Loads the level if it exists, otherwise toggles the Coming Soon panel.
    public void OnPress()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxArray[3]);
        string buttonText = _lvlButton.GetComponentInChildren<TMP_Text>().text;
        if (buttonText.Contains("Hub"))
        {
            _sceneSwitcher.LoadHub();
                        return;
        }
        if (int.TryParse(buttonText, out int result))
        {
            int levelNum = result;
            int prevLvlNum = levelNum - 1;
            string prevLevelID = prevLvlNum.ToString();

            if (Application.CanStreamedLevelBeLoaded("Level" + levelNum) && _levelSelect.IsUnlocked(prevLevelID))
            {
                _sceneSwitcher.LoadLevel(levelNum);
            }
            else if (Application.CanStreamedLevelBeLoaded("Level" + levelNum) && !_levelSelect.IsUnlocked(prevLevelID))
            {
                Debug.Log("Level is locked.");
                _levelSelect.ToggleUnlockLevelPanel();
            }
            else
            {
                Debug.LogWarning("Level doesn't exist or isn't in build settings.");
                _levelSelect.ToggleComingSoonPanel();
            }
        }
        else
        {
            Debug.LogError("What on earth is on that button. Where is the number");
        }
    }

    // Checks if a level is unlocked. If it's locked, the button gets greyed out.
    private void GreyOut()
    {
        string buttonText = _lvlButton.GetComponentInChildren<TMP_Text>().text;
        if (buttonText.Contains("Hub"))
        {
            return;
        }
        if (int.TryParse(buttonText, out int result))
        {
            int levelNum = result;
            int prevLvlNum = levelNum - 1;
            string prevLevelID = prevLvlNum.ToString();

            ColorBlock colors = _lvlButton.colors;
            if (!_levelSelect.IsUnlocked(prevLevelID))
            {
                //_lvlButton.interactable = false;
                colors.normalColor = Color.gray;
                colors.highlightedColor = Color.gray;
                colors.pressedColor = Color.gray;
                _lvlButton.GetComponentInChildren<TMP_Text>().color = Color.grey;
            }
            else if (_levelSelect.IsUnlocked(prevLevelID))
            {
                colors.normalColor = Color.white;
                colors.highlightedColor = Color.white;
                colors.pressedColor = Color.gray;
                _lvlButton.GetComponentInChildren<TMP_Text>().color = Color.white;
            }
            _lvlButton.colors = colors;
        }
    }
}
