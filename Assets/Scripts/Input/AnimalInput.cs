using UnityEngine;

// Script that handles all input related to animal interaction in the hub

public class AnimalInput : MonoBehaviour
{
    // FROM CHANDLER - animal information variables
    private Animal selectedAnimal;

    // FROM CHANDLER - interaction UI variables
    [SerializeField] private GameObject interactUI;
    private NeedsHUD needsUI;

    private void OnEnable()
    {
        // gets needs UI ref and closes UI if it's open
        needsUI = interactUI.GetComponent<NeedsHUD>();
        if (interactUI.activeSelf) interactUI.SetActive(false);
    }

    private void OnDisable() { }

    // CAMI - focus on a pressed animal
    public void FocusOnAnimal(Animal animal)
    {
        if (selectedAnimal != null && selectedAnimal != animal)
        {
            // If another animal is already selected, close its interaction UI
            UnfocusCurrAnimal();
        }

        selectedAnimal = animal;
        selectedAnimal.SetFocusState(true);

        if (selectedAnimal.TryGetComponent(out AnimalTween tween))
        { 
            tween.Zoom();
        }

        OpenInteractUI();
    }

    // CAMI - call to unfocus on currently selected animal
    public void UnfocusCurrAnimal()
    {
        if (selectedAnimal != null)
        {
            // if an animal is currently selected, close its interaction UI
            selectedAnimal.SetFocusState(false);

            if (selectedAnimal.TryGetComponent(out AnimalTween tween))
            {
                tween.Unzoom();
            }

            selectedAnimal = null;
        }
        CloseInteractUI();
    }

    // FROM CHANDLER - open interaction UI
    private void OpenInteractUI()
    {
        needsUI.SetAnimal(selectedAnimal.AnimalType);
        interactUI.SetActive(true);
    }

    // FROM CHANDLER - close interaction UI
    private void CloseInteractUI()
    {
        needsUI.SetAnimal(AnimalManager.AnimalType.none); // clears ref just in case
        interactUI.SetActive(false);
    }
}
