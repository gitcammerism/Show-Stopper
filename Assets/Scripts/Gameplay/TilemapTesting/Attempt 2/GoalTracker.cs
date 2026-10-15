using System;
using UnityEngine;

public class GoalTracker : MonoBehaviour
{
    [SerializeField] private LevelData level;
    public Color[] fillColors;
    public Color[] backgroundColors;

    private int[] collected;
    public event Action Changed;

    public int GoalCount => level.goals.Length;
    public TileGoal GetGoal(int i) => level.goals[i];
    public int GetCollected(int i) => collected[i];

    void Awake() => collected = new int[level.goals.Length];

    public void RecordCleared(int tileType)
    {
        bool changed = false;
        for (int i = 0; i < level.goals.Length; i++)
        {
            var g = level.goals[i];
            if (g.tileType == tileType && collected[i] < g.target)
            {
                collected[i]++;
                changed = true;
            }
        }
        if (changed) Changed?.Invoke();
    }

    public bool AllMet()
    {
        for (int i = 0; i < level.goals.Length; i++)
        {
            if (collected[i] < level.goals[i].target) return false;
        }
        return true;
    }
}
