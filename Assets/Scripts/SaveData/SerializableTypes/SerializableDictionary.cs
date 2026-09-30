using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    // Called before serializing
    // Saves dictionary to lists
    public void OnBeforeSerialize()
    {
        // Make sure lists are cleared
        keys.Clear();
        values.Clear();

        // Look through each KeyValuePair and add to list
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    // Called after deserializing
    // Loads dictionary from lists
    public void OnAfterDeserialize()
    {
        // Make sure dictionary is clear
        this.Clear();

        // Debug check to make sure loading has gone smoothly
        if (keys.Count != values.Count)
        {
            Debug.LogError("Error when deserializing the SerializableDictionary. " +
                "Amount of Keys (" + keys.Count + ") does not match values (" + values.Count + ")");

        }

        // Loop through and add each key to dictionary
        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }
}
