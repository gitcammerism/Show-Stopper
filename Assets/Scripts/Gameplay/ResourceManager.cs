using System;
using System.Collections.Generic;
using UnityEngine;

// This class handles the management and tracking of resources and the goals within the given level.

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private List<TileGoal> goalList;
    private Dictionary<TileState, int> _goalDictionary;
    public Dictionary<TileState, int> levelProgress;

    public static event Action OnGoalReached;

    // Since dictionaries aren't serializable, a struct list has to be used instead.
    void OnEnable()
    {
        _goalDictionary = new();
        levelProgress = new();
        
        foreach (TileGoal goal in goalList)
        {
            if (!_goalDictionary.ContainsKey(goal.tile))
            {
                _goalDictionary.Add(goal.tile, goal.goal);
                levelProgress.Add(goal.tile, 0);
                Debug.Log("Added " + goal.goal + " goal for " + goal.tile);
            }
        }
        
        // Subscribes to Match3Game's OnMatchMade event.
        Match3Game.OnMatchMade += CheckProgress;
    }

    void OnDisable()
    {
        Match3Game.OnMatchMade -= CheckProgress;
    }

    // Checks after each match if progress towards the goal has been made.
    private void CheckProgress(Match match)
    {
        // If the tile isn't in the dictionary, it's not a goal so we can return.
        if (!_goalDictionary.ContainsKey(match.tile)) return;
        
        // Add the length of the match to the tile amount.
        levelProgress[match.tile] += match.length;
        
        // If the progress of the given tile has been reached, check if the other tile goals have been reached.
        if (levelProgress[match.tile] >= _goalDictionary[match.tile])
        {
            foreach (TileState tile in _goalDictionary.Keys)
            {
                // If any tile is at a value less than the goal, immediately exit since it means goal is not reached.
                if (levelProgress[tile] < _goalDictionary[tile]) return;
            }

            Debug.Log("Goal complete!");
            OnGoalReached?.Invoke();
        }
    }
}
