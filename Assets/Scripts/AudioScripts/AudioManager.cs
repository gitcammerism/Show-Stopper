using UnityEngine;
using UnityEngine.Audio;

/**
 * This script manages setting up the audio sources and clips and playing them.
 */

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;

    public AudioClip bgMusic;
    public AudioClip testSFX;

    private AudioType _audioType;

    private void Awake()
    {
        // check if instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // start bg music, have it loop
        musicSource.clip = bgMusic;
        musicSource.Play();
        musicSource.loop = true;
    }


    // call from other scripts when sfx needs to play
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
