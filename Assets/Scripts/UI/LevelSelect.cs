using UnityEngine;

/*
 * This script is responsible for toggling the Coming Soon panel on and off in the Level Select scene.
 * This script can also be expanded to handle other Level Select scene management tasks.
 */

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private GameObject comingSoonPanel;


    private void OnEnable()
    {
        comingSoonPanel.SetActive(false);
    }

    public void ToggleComingSoonPanel()
    {
        comingSoonPanel.SetActive(!comingSoonPanel.activeSelf);
    }

}
