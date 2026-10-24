using UnityEngine;

/*
 * This script is responsible for managing tasks specific to the Level Select scene.
 * It plays the level select music and toggles the "coming soon" panel if player tries to open a nonexistent level.
 */

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private GameObject comingSoonPanel;
    [SerializeField] private AudioManager _audioManager;


    private void OnEnable()
    {
        comingSoonPanel.SetActive(false);
    }

    private void Start()
    {
        _audioManager = AudioManager.Instance;
        _audioManager.PlayMusic(_audioManager.bgMusicArray[1]);
    }

    public void ToggleComingSoonPanel()
    {
        comingSoonPanel.SetActive(!comingSoonPanel.activeSelf);
    }

}
