using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip levelMusic;
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
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        UpdateAudioSourceVolume();

        // Registrar cambios de escena para cambiar la música automáticamente
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // Comprobar la música que debe sonar en la escena actual al iniciar
        UpdateMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        // Limpiar el evento al destruir el objeto
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateMusicForScene(scene.name);
    }

    private void UpdateMusicForScene(string sceneName)
    {
        // Resetear la atenuación al cambiar de escena
        SetMusicDucking(false);

        AudioClip targetClip = null;

        // Lógica para decidir qué música poner según el nombre de la escena
        if (sceneName == "Menu" || sceneName == "LevelSelect")
        {
            targetClip = menuMusic;
        }
        else if (sceneName.Contains("Level") || sceneName == "Tutorial")
        {
            targetClip = levelMusic;
        }

        // Si hay un clip objetivo y no es el que ya está sonando, lo cambiamos
        if (targetClip != null && audioSource.clip != targetClip)
        {
            audioSource.clip = targetClip;
            audioSource.Play();
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || audioSource.clip == clip) return;
        
        audioSource.clip = clip;
        audioSource.Play();
    }

    private bool isDucked = false;

    private void UpdateAudioSourceVolume()
    {
        if (audioSource == null) return;
        float baseVolume = PlayerPrefs.GetFloat(VolumeKey, DefaultVolume);
        audioSource.volume = isDucked ? baseVolume * 0.4f : baseVolume;
    }

    public void SetMusicDucking(bool ducking)
    {
        isDucked = ducking;
        UpdateAudioSourceVolume();
    }

    public void SetVolume(float volume)
    {
        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.Save();
        UpdateAudioSourceVolume();
    }

    public float GetVolume()
    {
        return PlayerPrefs.GetFloat(VolumeKey, DefaultVolume);
    }

    public void SetPaused(bool paused)
    {
        if (audioSource == null) return;
        if (paused) audioSource.Pause();
        else audioSource.UnPause();
    }
}