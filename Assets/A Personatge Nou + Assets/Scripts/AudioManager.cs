using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioClip menuMusic;
    private AudioSource audioSource;

    private const string VolumeKey = "MusicVolume";
    private const float DefaultVolume = 0.75f;

    void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = menuMusic;
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, DefaultVolume);
        audioSource.volume = savedVolume;
        audioSource.Play();
    }

    
    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.Save();
    }

    
    public float GetVolume()
    {
        return audioSource.volume;
    }

    
    public void SetPaused(bool paused)
    {
        if (paused) audioSource.Pause();
        else audioSource.UnPause();
    }
}