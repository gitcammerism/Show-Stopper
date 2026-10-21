using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

/**
 * This script manages setting up the audio sources and clips and playing them.
 */

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;

    public AudioClip[] bgMusicArray;
    public AudioClip[] sfxArray;

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        // start main menu bg music, have it loop
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            musicSource.clip = bgMusicArray[0];
            musicSource.Play();
            musicSource.loop = true;
            Debug.Log("mainmenu music playing");
        }
        else
        {
            Debug.Log("mainmenu music not playing");

        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // stops main menu song in other scenes, plays again in main menu
        if (scene.name != "MainMenu")
        {
            musicSource.Stop();
        }
        else
        {
            musicSource.clip = bgMusicArray[0];
            musicSource.Play();
            musicSource.loop = true;
            Debug.Log("mainmenu music playing");
        }
    }


    // call from other scripts when sfx needs to play
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    // call from other scene manager scripts when a scene is loaded
    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
        musicSource.loop = true;
        Debug.Log($"playing {clip.name}");
    }
}
