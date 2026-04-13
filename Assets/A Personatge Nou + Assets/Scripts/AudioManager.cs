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
        // Patrón Singleton: destruye duplicados si ya existe
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

        // Carga el volumen guardado (o el valor por defecto)
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, DefaultVolume);
        audioSource.volume = savedVolume;
        audioSource.Play();
    }

    /// <summary>Cambia el volumen (0.0 - 1.0) y lo persiste.</summary>
    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.Save();
    }

    /// <summary>Devuelve el volumen actual para inicializar el slider.</summary>
    public float GetVolume()
    {
        return audioSource.volume;
    }

    /// <summary>Pausa/reanuda la música (útil si entras a un nivel con su propia música).</summary>
    public void SetPaused(bool paused)
    {
        if (paused) audioSource.Pause();
        else audioSource.UnPause();
    }
}