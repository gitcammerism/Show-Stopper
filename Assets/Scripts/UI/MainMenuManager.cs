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
        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxArray[3]);
        mainMenu.SetActive(true);
        settings.SetActive(false);
        howToPlay.SetActive(false);
        areYouSure.SetActive(false);
        credits.SetActive(false);
    }

    public void ToggleSettings()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxArray[3]);
        settings.SetActive(true);
        mainMenu.SetActive(false);
        howToPlay.SetActive(false);
        areYouSure.SetActive(false);
        credits.SetActive(false);
    }

    public void ToggleHowToPlay()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxArray[3]);
        settings.SetActive(false);
        mainMenu.SetActive(false);
        howToPlay.SetActive(true);
        areYouSure.SetActive(false);
        credits.SetActive(false);
    }
    public void ToggleAreYouSure()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxArray[3]);
        settings.SetActive(false);
        mainMenu.SetActive(false);
        howToPlay.SetActive(false);
        areYouSure.SetActive(true);
        credits.SetActive(false);
    }

    public void ToggleCredits()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxArray[3]);
        settings.SetActive(false);
        mainMenu.SetActive(false);
        howToPlay.SetActive(false);
        areYouSure.SetActive(false);
        credits.SetActive(true);
    }

    public void DeleteData()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxArray[3]);
        DataPersistenceManager.instance.DeleteData();
        ToggleMainMenu();
    }
}
