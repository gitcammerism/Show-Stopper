using System.Collections.Generic;
using UnityEngine;

/*
 * Last Modified: 09/16/2026 by Chandler Guzman
 * 
 * This is a struct that handles the spawning and despawning of match-3 tiles.
 *
 * Chandler TO-DO:
 * - 
 */

public struct TileInstancePool<T> where T : MonoBehaviour
{
    private Stack<T> _pool;

    // Creates a new prefab instance.
    public T GetInstance (T prefab)
    {
        // If the pool has not already been created, create it.
        if (_pool == null) _pool = new();

        // Ensures we're not referencing an old pool from previous play mode tests.
        # if UNITY_EDITOR 
        else if (_pool.TryPeek(out T i) && !i) _pool.Clear();
        #endif

        // If the prefab already exists, set it as active; otherwise, instantiate it
        if (_pool.TryPop(out T instance)) instance.gameObject.SetActive(true);
        else instance = Object.Instantiate(prefab);
        
        return instance;
    }

    // Recycles the given prefab instance.
    public void Recycle (T instance)
    {
        // If the pool is empty, destroy instance and abort.
        #if UNITY_EDITOR
        if (_pool == null)
        {
            Object.Destroy(instance.gameObject);
            return;
        }
        #endif

        // Sends the object back into the pool, then hides it.
        _pool.Push(instance);
        instance.gameObject.SetActive(false);
    }
}
