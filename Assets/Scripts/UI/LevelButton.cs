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

    // public constructor that creates a new level button
    // Cami TODO: maybe have buttons appear as they are loaded in the level select scene, instead of being preloaded in the scene.
    // Either that or have the buttons be greyed out
    //public LevelButton(int levelNum)
    //{
    //    _levelNum = levelNum;
    //    _lvlButton = this;
    //}

    private void Awake()
    {
        // Get the button component from the child object and grab the SceneSwitcher and LevelSelectManager
        _lvlButton = this.GetComponentInChildren<Button>();
        _sceneSwitcher = FindAnyObjectByType<SceneSwitcher>();
        _levelSelect = FindAnyObjectByType<LevelSelect>();
        _lvlButton.onClick.AddListener(OnPress);
    }

    // Called when the button is pressed. Loads the level if it exists, otherwise toggles the Coming Soon panel.
    public void OnPress()
    {
        string buttonText = _lvlButton.GetComponentInChildren<TMP_Text>().text;

        if (int.TryParse(buttonText, out int result))
        {
            int levelNum = result;

            if (Application.CanStreamedLevelBeLoaded("Level" + levelNum))
            {
                _sceneSwitcher.LoadLevel(levelNum);
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
}
