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
        masterVolScrollbar.SetValueWithoutNotify(masterVol);
        musicVolScrollbar.SetValueWithoutNotify(musicVol);
        sfxVolScrollbar.SetValueWithoutNotify(sfxVol);
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
}
