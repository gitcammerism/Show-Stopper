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

    // Toggles the Coming Soon panel on and off (positioned in this script unless a more suitable one is found)
    public void ToggleComingSoonPanel(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
