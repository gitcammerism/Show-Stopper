using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class Match3Board : MonoBehaviour
{
    [Header("Components")] 
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap boardTilemap;

    [Header("Prefabs")] 
    public Tile[] tilePrefabs;
    
    private static Match3Board _instance;
    private Dictionary<Vector3Int, Tile> _boardTiles = new();

    void Awake()
    {
        _instance = this;
    }
    
    void Start()
    {
        // Refreshes all tiles, ensuring that the preview sprites won't be seen in-game.
        #if !UNITY_EDITOR
        var tilemaps = grid.GetComponentsInChildren<Tilemap>();
        foreach (var tilemap in tilemaps)
        {
            tilemap.RefreshAllTiles();
        }
        #endif
    }

    // Spawns a tile at the given position (called from TilePlacer).
    public static void SpawnTile(Vector3Int position, Tile tile = null)
    {
        // Error-handling for null instance
        if (_instance == null) 
            _instance = GameObject.Find("Grid").GetComponent<Match3Board>();
        
        // Adds to dictionary
        if (!_instance._boardTiles.ContainsKey(position)) 
            _instance._boardTiles.Add(position, tile);
        
        // Spawns new tile prefab at given position
        Tile newTile = Instantiate(
            _instance.tilePrefabs[Random.Range(0, _instance.tilePrefabs.Length)], 
    _instance.grid.GetCellCenterWorld(position), 
            Quaternion.identity
            );
        
        // Stores prefab in dictionary
        _instance._boardTiles[position] = newTile;
    }
}
