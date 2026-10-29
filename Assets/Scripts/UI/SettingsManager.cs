using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/**
 * This script manages the changing values of the scrollbars in Settings and sets the audio volume accordingly.
 */


public class SettingsManager : MonoBehaviour
{
    private AudioManager _audioManager;

    public float masterVol, musicVol, sfxVol = 0f;

    [SerializeField]
    private AudioMixer _mainMixer;
    //private AudioMixerGroup mainAudioMixerGroup, musicAudioMixerGroup, sfxAudioMixerGroup;

    [SerializeField]
    public Scrollbar masterVolScrollbar, musicVolScrollbar, sfxVolScrollbar;

    // For Hints
    private const string HintsEnabledKey = "HintsEnabled";

    public static event Action<bool> OnHintsChanged;

    private void Awake()
    {
        _audioManager = AudioManager.Instance;
    }
    public static bool IsEnabled
    {
        get
        {
            return PlayerPrefs.GetInt(HintsEnabledKey, 1) == 1;
        }
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
        _mainMixer.SetFloat("master", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20);
    }

    public void AddToMasterVolume()
    {
        if (masterVolScrollbar.value < 1f)
        {
            if (masterVolScrollbar.value + 0.2f > 1f)
            {
                masterVolScrollbar.value = 1f;
            }
            else
            {
                masterVolScrollbar.value += 0.2f;
            }
            ChangeMasterVolume();
        }
    }

    public void SubtractFromMasterVolume()
    {
        if (masterVolScrollbar.value > 0f)
        {
            if (masterVolScrollbar.value - 0.2f < 0f)
            {
                masterVolScrollbar.value = 0.0001f;
            }
            else
            {
                masterVolScrollbar.value -= 0.2f;
            }
            ChangeMasterVolume();
        }
    }

    public void SetMusicVolume()
    {
        float volume = musicVolScrollbar.value;
        _mainMixer.SetFloat("music", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20);
    }

    public void AddToMusicVolume()
    {
        if (musicVolScrollbar.value < 1f)
        {
            if (musicVolScrollbar.value + 0.2f > 1f)
            {
                musicVolScrollbar.value = 1f;
            }
            else
            {
                musicVolScrollbar.value += 0.2f;
            }
            SetMusicVolume();
        }
    }

    public void SubtractFromMusicVolume()
    {
        if (musicVolScrollbar.value > 0f)
        {
            if (musicVolScrollbar.value - 0.2f < 0f)
            {
                musicVolScrollbar.value = 0.0001f;
            }
            else
            {
                musicVolScrollbar.value -= 0.2f;
            }
            SetMusicVolume();
        }
    }

    public void SetSFXVolume()
    {
        float volume = sfxVolScrollbar.value;
        _mainMixer.SetFloat("sfx", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20);
    }

    public void AddToSFXVolume()
    {
        if (sfxVolScrollbar.value < 1f)
        {
            if (sfxVolScrollbar.value + 0.2f > 1f)
            {
                sfxVolScrollbar.value = 1f;
            }
            else
            {
                sfxVolScrollbar.value += 0.2f;
            }
            SetSFXVolume();
        }
    }

    public void SubtractFromSFXVolume()
    {
        if (sfxVolScrollbar.value > 0f)
        {
            if (sfxVolScrollbar.value - 0.2f < 0f)
            {
                sfxVolScrollbar.value = 0.0001f;
            }
            else
            {
                sfxVolScrollbar.value -= 0.2f;
            }
            SetSFXVolume();
        }
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

    // COLIN - Change bool to enable or disabled
    public void SetHintsEnabled(bool enabled)
    {
        Debug.Log("Toggle passed this value: " + enabled);

        PlayerPrefs.SetInt(HintsEnabledKey, enabled ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("Saved integer: " +
            PlayerPrefs.GetInt(HintsEnabledKey, -1));

        Debug.Log("IsEnabled returns: " + IsEnabled);

        OnHintsChanged?.Invoke(enabled);
    }
}
