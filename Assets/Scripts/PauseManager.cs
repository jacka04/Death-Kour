using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("Panels")]
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Controller Support")]
    [SerializeField] private GameObject firstButtonPause;
    [SerializeField] private GameObject firstButtonOptions;

    [Header("Escenas")]
    [SerializeField] private string mainMenuSceneName = "Menu";
    [SerializeField] private string nextLevelSceneName;

    [Header("Sonidos")]
    [SerializeField] private AudioClip openMenuSound;
    [SerializeField] private AudioClip closeMenuSound;
    private AudioSource audioSource;

    [Header("Animación")]
    [SerializeField] private float animationDuration = 0.2f;
    private CanvasGroup canvasGroup;
    private Coroutine animationCoroutine;

    private bool isPaused = false;

    private void Awake()
    {
        Instance = this;
        pauseCanvas.SetActive(false); 
        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);

        // Añadir AudioSource para los sonidos del menú
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // Sonido 2D

        // Configurar CanvasGroup para el fade
        canvasGroup = pauseCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = pauseCanvas.AddComponent<CanvasGroup>();
    }

    private void Update()
    {
        // Abrir con ESC o con el botón "Menu/Options" del mando Xbox (JoystickButton7)
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
            TogglePause();
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        if (AudioManager.Instance != null) AudioManager.Instance.SetMusicDucking(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;
        
        pauseCanvas.SetActive(true);
        pausePanel.SetActive(true);
        optionsPanel.SetActive(false);

        // Seleccionar primer botón para mando
        if (firstButtonPause != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstButtonPause);
        }

        // Iniciar animación de entrada
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(AnimateMenu(true));

        if (openMenuSound != null) audioSource.PlayOneShot(openMenuSound);
        if (AudioManager.Instance != null) AudioManager.Instance.SetMusicDucking(true);
    }

    public void Resume()
    {
        if (!isPaused) return;

        isPaused = false;
        
        // Iniciar animación de salida
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(AnimateMenu(false));

        if (closeMenuSound != null) audioSource.PlayOneShot(closeMenuSound);
        if (AudioManager.Instance != null) AudioManager.Instance.SetMusicDucking(false);
    }

    private IEnumerator AnimateMenu(bool opening)
    {
        float timer = 0f;
        Vector3 startScale = opening ? Vector3.zero : Vector3.one;
        Vector3 endScale   = opening ? Vector3.one  : Vector3.zero;
        float startAlpha   = opening ? 0f : 1f;
        float endAlpha     = opening ? 1f : 0f;

        pausePanel.transform.localScale = startScale;
        canvasGroup.alpha = startAlpha;

        while (timer < animationDuration)
        {
            timer += Time.unscaledDeltaTime; // Importante usar unscaled porque el tiempo está pausado
            float t = Mathf.Clamp01(timer / animationDuration);
            
            // Suavizado de la animación (Ease Out Quad)
            float easedT = t * (2f - t);

            pausePanel.transform.localScale = Vector3.Lerp(startScale, endScale, easedT);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, easedT);
            yield return null;
        }

        pausePanel.transform.localScale = endScale;
        canvasGroup.alpha = endAlpha;

        if (!opening)
        {
            pauseCanvas.SetActive(false);
            pausePanel.SetActive(false);
            Time.timeScale = 1f; // Restaurar el tiempo al terminar de cerrar
        }
        
        animationCoroutine = null;
    }

    public void OpenOptions()
    {
        pausePanel.SetActive(false);
        optionsPanel.SetActive(true);

        if (firstButtonOptions != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstButtonOptions);
        }
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        pausePanel.SetActive(true);

        if (firstButtonPause != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstButtonPause);
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextLevelSceneName))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nextLevelSceneName);
        }
        else
        {
            Debug.LogWarning("¡No se ha especificado el nombre del siguiente nivel en el PauseManager!");
        }
    }
}