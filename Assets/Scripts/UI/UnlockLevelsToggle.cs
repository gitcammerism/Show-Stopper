using UnityEngine;
using UnityEngine.UI;

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
