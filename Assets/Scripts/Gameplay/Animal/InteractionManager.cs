using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] private AnimalInput animalInput;
    [SerializeField] private InventorySlot[] inventory;

    private Animal animal;

    private void OnEnable()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i].itemButton.onClick.AddListener(() => Debug.Log("Button clicked!"));
        }
        animal = animalInput.SelectedAnimal;
        
    }

    private void OnDisable()
    {
        animal = null;
    }

    private void OnTouch()
    {
        Debug.Log("Clicked");
    }
}
