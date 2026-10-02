using UnityEngine.UI;

// This struct contains the information for the match-3 level goals (used by ResourceManager).

[System.Serializable]
public class TileGoal
{
    public TileState tile;
    public int goal;
    public Slider goalSlider;
}
