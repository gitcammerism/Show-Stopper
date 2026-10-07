using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioClip[] sounds;
    [SerializeField] private AudioClip music;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
