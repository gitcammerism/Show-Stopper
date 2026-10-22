using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject howToPlay;
    [SerializeField] private GameObject areYouSure;
    [SerializeField] private GameObject credits;
    void OnEnable()
    {
        mainMenu.SetActive(true);
        settings.SetActive(false);
        howToPlay.SetActive(false);
        areYouSure.SetActive(false);
        credits.SetActive(false);
    }

    public void ToggleMainMenu()
    {
        mainMenu.SetActive(true);
        settings.SetActive(false);
        howToPlay.SetActive(false);
        areYouSure.SetActive(false);
        credits.SetActive(false);
    }

    public void ToggleSettings()
    {
        settings.SetActive(true);
        mainMenu.SetActive(false);
        howToPlay.SetActive(false);
        areYouSure.SetActive(false);
        credits.SetActive(false);
    }

    public void ToggleHowToPlay()
    {
        settings.SetActive(false);
        mainMenu.SetActive(false);
        howToPlay.SetActive(true);
        areYouSure.SetActive(false);
        credits.SetActive(false);
    }
    public void ToggleAreYouSure()
    {
        settings.SetActive(false);
        mainMenu.SetActive(false);
        howToPlay.SetActive(false);
        areYouSure.SetActive(true);
        credits.SetActive(false);
    }

    public void ToggleCredits()
    {
        settings.SetActive(false);
        mainMenu.SetActive(false);
        howToPlay.SetActive(false);
        areYouSure.SetActive(false);
        credits.SetActive(true);
    }
}
