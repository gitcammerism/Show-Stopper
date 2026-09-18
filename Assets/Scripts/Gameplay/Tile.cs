using UnityEngine;

/*
 * Last Modified: 09/16/2026 by Chandler Guzman
 * 
 * This is a script that handles the spawning and despawning of the assigned tile.
 *
 * Chandler TO-DO:
 * - 
 */

public class Tile : MonoBehaviour
{
    private TileInstancePool<Tile> _pool;

    // Gets an instance of itself, assigns self a pool, and places tile at given position
    public Tile Spawn (Vector3 position)
    {
        Tile instance = _pool.GetInstance(this);
        instance._pool = _pool;
        instance.transform.localPosition = position;
        return instance;
    }

    // Recycles the given tile
    public void Despawn () => _pool.Recycle(this);
}
