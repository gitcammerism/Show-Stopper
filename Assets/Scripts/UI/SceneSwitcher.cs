using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Last Modified: 09/28/2026 by Summer Smith
 * 
 * This script handles the scene switching inside of the game
 */

public class SceneSwitcher : MonoBehaviour
{
    
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    // Animal hub scene
    public void LoadHub()
    {
        SceneManager.LoadScene("HubLevel");
    }

    // Loads the level based on the level number as selected in the level select
    public void LoadLevel(int levelNum)
    {
        SceneManager.LoadScene("Level" + levelNum);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
