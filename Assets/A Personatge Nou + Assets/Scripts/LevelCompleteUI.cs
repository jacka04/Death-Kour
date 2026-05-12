using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// Gestiona el canvas de nivel completado y el sistema de estrellas.
// Las estrellas se calculan en función del tiempo restante sobre el tiempo total.
public class LevelCompleteUI : MonoBehaviour
{
    public static LevelCompleteUI Instance;

    [Header("Canvas principal")]
    [SerializeField] private GameObject levelCompleteCanvas;
    [SerializeField] private Image fadePanel;
    [SerializeField] private GameObject completePanel;

    [Header("Estrellas")]
    [SerializeField] private Image[] starImages;          // 3 imágenes de estrella, en orden
    [SerializeField] private Sprite starFilledSprite;     // Sprite estrella llena
    [SerializeField] private Sprite starEmptySprite;      // Sprite estrella vacía

    [Header("Umbrales de estrella (% de tiempo restante)")]
    [Tooltip("Porcentaje mínimo de tiempo restante para 3 estrellas. Ej: 0.5 = 50%")]
    [Range(0f, 1f)] [SerializeField] private float threshold3Stars = 0.50f;
    [Tooltip("Porcentaje mínimo de tiempo restante para 2 estrellas. Ej: 0.25 = 25%")]
    [Range(0f, 1f)] [SerializeField] private float threshold2Stars = 0.25f;

    [Header("Texto informativo")]
    [SerializeField] private TextMeshProUGUI timeLeftText;   // Opcional: muestra el tiempo sobrante
    [SerializeField] private TextMeshProUGUI starsLabelText; // Opcional: mensaje según estrellas

    [Header("Objetos Extra")]
    [Tooltip("Textos u objetos que deben aparecer junto con el panel final")]
    [SerializeField] private GameObject[] extraElements;

    [Header("Sonidos")]
    [SerializeField] private AudioClip[] starPopClips;  // Un clip para cada estrella
    [SerializeField] [Range(0f, 1f)] private float starPopVolume = 0.8f;
    private AudioSource audioSource;

    [Header("Tiempos de animación")]
    [SerializeField] private float fadeDuration    = 0.6f;
    [SerializeField] private float panelPopDuration = 0.4f;
    [SerializeField] private float starDelay       = 0.25f;  // Retardo entre cada estrella
    [SerializeField] private float starPopDuration = 0.3f;
    [SerializeField] private float starFinalScale  = 1.0f; // Nueva variable de tamaño

    private void Awake()
    {
        Instance = this;
        levelCompleteCanvas.SetActive(false);
        completePanel.SetActive(false);

        // Ocultar elementos extra inicialmente
        if (extraElements != null)
        {
            foreach (var obj in extraElements)
                if (obj != null) obj.SetActive(false);
        }

        // Configurar audio
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // Sonido 2D para UI

        // Empezar todas las estrellas apagadas y escala 0 para que puedan "popear"
        foreach (var star in starImages)
        {
            star.sprite = starEmptySprite;
            star.rectTransform.localScale = Vector3.zero;
        }
    }

    // Llamado desde GoalTrigger cuando el jugador llega a la meta
    public void ShowLevelComplete()
{
    levelCompleteCanvas.SetActive(true); 
    StartCoroutine(LevelCompleteSequence());
}

    private IEnumerator LevelCompleteSequence()
    {
        levelCompleteCanvas.SetActive(true);

        // Fade a negro
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));
        // Fade a transparente
        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

        // Mostrar panel con animación de pop
        completePanel.SetActive(true);

        // Mostrar elementos extra (los que aparecían "de golpe")
        if (extraElements != null)
        {
            foreach (var obj in extraElements)
                if (obj != null) obj.SetActive(true);
        }

        yield return StartCoroutine(AnimatePanel());

        // Calcular estrellas
        int stars = CalculateStars();

        // Mostrar textos opcionales
        if (timeLeftText != null && GameTimer.Instance != null)
        {
            int seconds      = (int)GameTimer.Instance.TimeLeft;
            int milliseconds = (int)((GameTimer.Instance.TimeLeft - seconds) * 100);
            timeLeftText.text = $"Tiempo restante: {seconds:00}:{milliseconds:00}";
        }

        if (starsLabelText != null)
        {
            starsLabelText.text = stars switch
            {
                3 => "¡Perfecto!",
                2 => "¡Bien hecho!",
                _ => "¡Lo lograste!"
            };
        }

        // Animar estrellas una a una
        yield return StartCoroutine(AnimateStars(stars));
    }

    // Devuelve 1, 2 o 3 estrellas según el tiempo restante
    private int CalculateStars()
    {
        if (GameTimer.Instance == null) return 1;

        float ratio = GameTimer.Instance.TimeLeft / GameTimer.Instance.TotalTime;

        if (ratio >= threshold3Stars) return 3;
        if (ratio >= threshold2Stars) return 2;
        return 1;
    }

    // Anima las estrellas encendiéndose de izquierda a derecha
    private IEnumerator AnimateStars(int count)
    {
        for (int i = 0; i < starImages.Length; i++)
        {
            yield return new WaitForSeconds(starDelay);

            // Cambiar sprite según si se ha ganado la estrella o no
            if (i < count)
            {
                starImages[i].sprite = starFilledSprite;
            }
            else
            {
                starImages[i].sprite = starEmptySprite;
            }
            
            // Reproducir sonido de estrella (cada una puede tener el suyo)
            if (starPopClips != null && i < starPopClips.Length && starPopClips[i] != null)
            {
                audioSource.PlayOneShot(starPopClips[i], starPopVolume);
            }

            // Todas las estrellas hacen la animación de Pop
            yield return StartCoroutine(PopStar(starImages[i].rectTransform));
        }
    }

    // Pop de escala para cada estrella
    private IEnumerator PopStar(RectTransform rect)
    {
        float timer = 0f;
        Vector3 finalScale = Vector3.one * starFinalScale;
        Vector3 overshoot  = finalScale * 1.3f;

        // Scale up con overshoot
        while (timer < starPopDuration * 0.6f)
        {
            timer += Time.deltaTime;
            float t = timer / (starPopDuration * 0.6f);
            rect.localScale = Vector3.Lerp(Vector3.zero, overshoot, t);
            yield return null;
        }

        // Scale back al tamaño normal
        timer = 0f;
        while (timer < starPopDuration * 0.4f)
        {
            timer += Time.deltaTime;
            float t = timer / (starPopDuration * 0.4f);
            rect.localScale = Vector3.Lerp(overshoot, finalScale, t);
            yield return null;
        }

        rect.localScale = finalScale;
    }

    // Pop del panel completo (igual que en GameTimer)
    private IEnumerator AnimatePanel()
    {
        float timer = 0f;
        RectTransform rect = completePanel.GetComponent<RectTransform>();
        Vector3 targetScale = new Vector3(0.85f, 0.85f, 1f);

        while (timer < panelPopDuration)
        {
            timer += Time.deltaTime;
            float t = timer / panelPopDuration;
            t = 1f - Mathf.Pow(1f - t, 3f);
            rect.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            yield return null;
        }

        rect.localScale = targetScale;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float timer = 0f;
        Color c = fadePanel.color;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, timer / duration);
            fadePanel.color = c;
            yield return null;
        }
        c.a = to;
        fadePanel.color = c;
    }

    // Botones del canvas
    public void OnNextLevelButton()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            SceneManager.LoadScene(0); // Volver al menú si no hay más niveles
    }

    public void OnRetryButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnMainMenuButton()
    {
        SceneManager.LoadScene(0);
    }
}