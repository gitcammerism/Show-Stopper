using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

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
