using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;


public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    public float masterVol, musicVol, sfxVol = 0f;

    [SerializeField]
    private AudioMixerGroup mainAudioMixerGroup, musicAudioMixerGroup, sfxAudioMixerGroup;

    public Scrollbar masterVolScrollbar, musicVolScrollbar, sfxVolScrollbar;

    [SerializeField]
    private AudioResource[] sounds;
    [SerializeField]
    private AudioResource music;

    private AudioType _audioType;

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
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        masterVolScrollbar.SetValueWithoutNotify(masterVol);
        musicVolScrollbar.SetValueWithoutNotify(musicVol);
        sfxVolScrollbar.SetValueWithoutNotify(sfxVol);
    }

    public void UpdateMasterVolume(float value)
    {
        masterVol = value;
        mainAudioMixerGroup.audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVol) * 20);
    }
}
