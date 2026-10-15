using UnityEngine;

[CreateAssetMenu(menuName = "Match 3/Level Data")]
public class LevelData : ScriptableObject
{
    public float timeLimit = 60f;
    public TileGoal[] goals = new TileGoal[1];

    void OnValidate()
    {
        // enforces up to 3 goals
        if (goals.Length > 3) System.Array.Resize(ref goals, 3);
    }
}
