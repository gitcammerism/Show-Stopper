using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private Dictionary<TileState, int> levelGoal;
    [System.NonSerialized] public Dictionary<TileState, int> levelProgress;
    private bool _goalReached = false;

    void Start()
    {
        levelGoal = new();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
