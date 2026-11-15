using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Last Modified: 09/28/2026 by Summer Smith
 * 
 * This script handles the scene switching inside of the game
 */

public class SceneSwitcher : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    public void LoadMainMenu()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxArray[3]);
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLevelSelect()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxArray[3]);
        SceneManager.LoadScene("LevelSelect");
    }

    // Animal hub scene
    public void LoadHub()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxArray[3]);
        SceneManager.LoadScene("HubLevel");
    }

    // Loads the level based on the level number as selected in the level select
    public void LoadLevel(int levelNum)
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxArray[3]);
        SceneManager.LoadScene("Level" + levelNum);
    }

    public void LoadNextLevel()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxArray[3]);
        if (int.TryParse(gameManager.levelID, out int levelNum))
        {
            levelNum++;
            SceneManager.LoadScene("Level" + levelNum.ToString());
        }
    }

    public void QuitGame()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxArray[3]);
        Application.Quit();
    }
}
