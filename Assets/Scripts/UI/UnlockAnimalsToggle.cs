using UnityEngine;
using UnityEngine.UI;

// unlocks/locks all animals in the game when toggle is set to true

public class UnlockAnimalsToggle : MonoBehaviour
{
    /* on start, get toggle component script is attached to & set to on or off depending on variable 
       (without triggering OnValueChanged) */
    private void Start() =>
    GetComponent<Toggle>().SetIsOnWithoutNotify(DataPersistenceManager.instance.areAllAnimalsUnlocked);

    // toggles all animals being unlocked or not (triggered OnValueChanged)
    public void ToggleUnlock(bool isUnlocked) =>
    DataPersistenceManager.instance.ToggleUnlockAllAnimals(isUnlocked);
}