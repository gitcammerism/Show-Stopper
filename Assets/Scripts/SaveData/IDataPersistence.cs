using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence
{
    // Takes in game data, read only
    void LoadData(GameData data);

    // Takes in reference to game data, can be modified
    void SaveData(ref GameData data);
}
