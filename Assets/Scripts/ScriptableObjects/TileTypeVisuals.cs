using System;
using UnityEngine;

// Object holding the tile visuals for the radial fill UI element

[CreateAssetMenu(menuName = "UI/Tile Type Visuals")]
public class TileTypeVisuals : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public TileType type;
        public Color color;
        public Sprite icon;
    }

    public Entry[] entries;

    public bool TryGet(TileType type, out Entry result)
    {
        foreach (var e in entries)
        {
            if (e.type == type) { result = e; return true; }
        }
        result = default;
        return false;
    }
}
