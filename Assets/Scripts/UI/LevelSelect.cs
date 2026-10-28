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



    private void OnEnable()
    {
        comingSoonPanel.SetActive(false);
    }

    private void Start()
    {
        _audioManager = AudioManager.Instance;
        _audioManager.PlayMusic(_audioManager.bgMusicArray[1]);
    }

    // cami todo: implement with levels
    public bool IsLocked()
    {
        return true;
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
