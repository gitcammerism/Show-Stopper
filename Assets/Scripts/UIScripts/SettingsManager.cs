using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;

/**
 * This script manages the changing values of the scrollbars in Settings and sets the audio volume accordingly.
 */


public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;
    private AudioManager _audioManager;

    public float masterVol, musicVol, sfxVol = 0f;

    [SerializeField]
    private AudioMixer _mainMixer;
    //private AudioMixerGroup mainAudioMixerGroup, musicAudioMixerGroup, sfxAudioMixerGroup;

    [SerializeField]
    public Scrollbar masterVolScrollbar, musicVolScrollbar, sfxVolScrollbar;

    private void Awake()
    {
        // check if instance exists
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _audioManager = AudioManager.Instance;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // COLIN - Load saved volume values
        masterVol = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

        masterVolScrollbar.SetValueWithoutNotify(masterVol);
        musicVolScrollbar.SetValueWithoutNotify(musicVol);
        sfxVolScrollbar.SetValueWithoutNotify(sfxVol);

        // Apply saved volumes
        ChangeMasterVolume();
        SetMusicVolume();
        SetSFXVolume();
    }

    public void ChangeMasterVolume()
    {
        float volume = masterVolScrollbar.value;
        _mainMixer.SetFloat("master", Mathf.Log10(volume)*20);
    }

    public void SetMusicVolume()
    {
        float volume = musicVolScrollbar.value;
        _mainMixer.SetFloat("music", Mathf.Log10(volume)*20);
    }

    public void SetSFXVolume()
    {
        float volume = sfxVolScrollbar.value;
        _mainMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
    }

    // COLIN - Saves current Audio settings
    public void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolScrollbar.value);
        PlayerPrefs.SetFloat("MusicVolume", musicVolScrollbar.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolScrollbar.value);

        PlayerPrefs.Save();

        Debug.Log("Audio settings saved.");
    }

    // COLIN - Resets all Audio settings back to default
    public void ResetAudioSettings()
    {
        // Default values
        masterVol = 1f;
        musicVol = 1f;
        sfxVol = 1f;

        // Update scrollbars
        masterVolScrollbar.SetValueWithoutNotify(masterVol);
        musicVolScrollbar.SetValueWithoutNotify(musicVol);
        sfxVolScrollbar.SetValueWithoutNotify(sfxVol);

        // Apply default values
        ChangeMasterVolume();
        SetMusicVolume();
        SetSFXVolume();

        // Delete saved PlayerPrefs
        PlayerPrefs.DeleteKey("MasterVolume");
        PlayerPrefs.DeleteKey("MusicVolume");
        PlayerPrefs.DeleteKey("SFXVolume");

        PlayerPrefs.Save();

        Debug.Log("Audio settings reset");
    }
}
