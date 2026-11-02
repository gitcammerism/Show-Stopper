using UnityEngine;
using UnityEngine.UI;

// This script locks/unlocks all levels in the game when the toggle it is attached to is clicked. 

public class UnlockLevelsToggle : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Toggle>().SetIsOnWithoutNotify(DataPersistenceManager.instance.areAllLevelsUnlocked);
    }

    public void ToggleUnlock(bool isUnlocked)
    {
        DataPersistenceManager.instance.ToggleUnlockAllLevels(isUnlocked);
    }
}
