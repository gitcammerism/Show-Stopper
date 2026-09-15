using UnityEngine;

/*
 * Last Modified: 09/15/2026 by Chandler Guzman
 *
 * This script creates a random tile from the given tile prefabs.
 *
 * Chandler TO-DO:
 * - Improve random tile generation (so no matches are instantly made)
 */

public class BackgroundTile : MonoBehaviour
{
    [SerializeField] private GameObject[] tilePrefabs;
    
    // When the tile is created, pick a random tile to spawn
    void Start()
    {
        PickTile();
    }

    // Picks and creates a random tile from the tilePrefabs array
    private void PickTile()
    {
        int tileToUse = Random.Range(0, tilePrefabs.Length);
        GameObject tile = Instantiate(tilePrefabs[tileToUse], transform.position, Quaternion.identity);
        tile.transform.SetParent(transform);
        tile.name = tilePrefabs[tileToUse].name;
    }
}
