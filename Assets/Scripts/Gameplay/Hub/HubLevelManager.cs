using UnityEngine;

/*
 * This script is responsible for management of the Hub level. At the moment it only plays the level music.
 */

public class HubLevelManager : MonoBehaviour
{
    private AudioManager _audioManager;

    void Start()
    {
        _audioManager = AudioManager.Instance;
        _audioManager.PlayMusic(_audioManager.bgMusicArray[2]);
    }
}
