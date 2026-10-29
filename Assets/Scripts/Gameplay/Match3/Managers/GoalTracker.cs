using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalTracker : MonoBehaviour
{
    [SerializeField] private LevelData level;
    public Sprite[] fillImages;
    public Sprite[] backgroundImages;
    public Color[] fillColors;
    public Color[] backgroundColors;

    private int[] collected;
    public event Action Changed;

    public int GoalCount => level.goals.Length;
    public TileGoal GetGoal(int i) => level.goals[i];
    public int GetCollected(int i) => collected[i];

    void Awake() => collected = new int[level.goals.Length];

    public void RecordCleared(TileType tileType)
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

    // FROM COLIN - Accessor to make saving collected resources easier
    public Dictionary<TileType, int> GetCollectedResources()
    {
        Dictionary<TileType, int> resources = new Dictionary<TileType, int>();

        for (int i = 0; i < level.goals.Length; i++)
        {
            TileType tileType = level.goals[i].tileType;
            int amount = collected[i];

            if (resources.ContainsKey(tileType))
            {
                resources[tileType] += amount;
            }
            else
            {
                resources.Add(tileType, amount);
            }
        }

        return resources;
    }

    // FROM COLIN - returns what tile type is needed to complete level
    // FROM COLIN - for hint system
    public bool NeedsMoreOf(TileType tileType)
    {
        for (int i = 0; i < level.goals.Length; i++)
        {
            TileGoal goal = level.goals[i];

            if (goal.tileType == tileType && collected[i] < goal.target)
            {
                return true;
            }
        }

        return false;
    }
}
