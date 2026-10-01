using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private Dictionary<TileState, int> levelGoal;
    [System.NonSerialized] public Dictionary<TileState, int> levelProgress;
    private bool _goalReached = false;

    void OnEnable()
    {
        levelGoal = new();
        Match3Game.OnMatchMade += CheckProgress;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CheckProgress(Match match)
    {
        if (!levelGoal.ContainsKey(match.tile)) return;


    }
}
