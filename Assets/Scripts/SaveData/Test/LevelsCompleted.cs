using TMPro;
using UnityEngine;

public class LevelsCompleted : MonoBehaviour, IDataPersistence
{
    public bool levelCompleted = false;

    // For generating unique level ids
    [SerializeField] private string ID;

    [ContextMenu("Generate Guid for ID")]
    private void GenerateGuid()
    {
        ID = System.Guid.NewGuid().ToString();
    }

    // Taken from IDataPersistence
    // Sees if the level has been completed
    public void LoadData(GameData data)
    {
        data.levelsCompleted.TryGetValue(ID, out levelCompleted);
    }

    public void SaveData(ref GameData data)
    {
        if (data.levelsCompleted.ContainsKey(ID))
        {
            data.levelsCompleted.Remove(ID);
        }

        data.levelsCompleted.Add(ID, levelCompleted);
    }
}
