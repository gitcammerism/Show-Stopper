using UnityEngine;
using UnityEngine.UI;

// struct which stores a button + tiletype combo

[System.Serializable]
public struct InventorySlot
{
    public Button itemButton;
    public TileType itemType;
}
