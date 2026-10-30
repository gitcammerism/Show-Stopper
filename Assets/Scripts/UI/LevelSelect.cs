using TMPro;
using UnityEngine;

/*
 * This script is responsible for managing tasks specific to the Level Select scene.
 * It plays the level select music and toggles the "coming soon" panel if player tries to open a nonexistent level.
 */

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private GameObject comingSoonPanel;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private TMP_Text _panelText;

    private GameData _gameData;


    private void OnEnable()
    {
        comingSoonPanel.SetActive(false);
    }

    private void Start()
    {
        _audioManager = AudioManager.Instance;
        _audioManager.PlayMusic(_audioManager.bgMusicArray[1]);
        _gameData = DataPersistenceManager.instance.GetGameData();
    }

    // checks if a level is locked or unlocked
    public bool IsUnlocked(string level)
    {
        if (DataPersistenceManager.instance.areAllLevelsUnlocked) return true;
        else if (level.Equals("0"))
        {
            return true;
        }
        else if (_gameData != null && _gameData.levelsCompleted.ContainsKey(level))
        {
            return _gameData.levelsCompleted[level];
        }
        else if (_gameData != null && _gameData.levelsCompleted.ContainsKey(level) == false)
        {
            return false;
        }

        Debug.Log("level select went wrong");
        return false;
    }

    public void ToggleComingSoonPanel()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxArray[3]);
        comingSoonPanel.SetActive(!comingSoonPanel.activeSelf);
        _panelText.text = "Coming Soon!";
    }

    public void ToggleUnlockLevelPanel()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxArray[3]);
        comingSoonPanel.SetActive(!comingSoonPanel.activeSelf);
        _panelText.text = "Complete the previous level(s) to unlock this one!";
    }

}
