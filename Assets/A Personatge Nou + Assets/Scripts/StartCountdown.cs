using System.Collections;
using UnityEngine;
using TMPro;

public class StartCountdown : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GameObject countdownPanel;

    [Header("Audio")]
    [SerializeField] private AudioClip tickSound;
    [SerializeField] private AudioClip goSound;
    [SerializeField] private AudioSource audioSource;

    [Header("Settings")]
    [SerializeField] private CelestePlayer player;
    [SerializeField] private GameTimer gameTimer;

    private void Awake()
    {
        // Asegurarse de tener un AudioSource configurado correctamente
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // Sonido 2D
        }
        
        // El AudioSource debe poder sonar incluso si el juego está en pausa
        audioSource.ignoreListenerPause = true;
    }

    private IEnumerator Start()
    {
        // 1. Congelar el juego
        // Usamos Time.timeScale = 0 para "congelar" físicas y animaciones que usen tiempo delta
        Time.timeScale = 0f;
        
        // También deshabilitamos el script del jugador para evitar que reciba input
        if (player != null) player.enabled = false;
        
        // Aseguramos que el panel esté activo
        if (countdownPanel != null) countdownPanel.SetActive(true);
        if (countdownText != null) countdownText.gameObject.SetActive(true);

        // 2. Cuenta atrás: 3, 2, 1
        for (int i = 3; i > 0; i--)
        {
            yield return StartCoroutine(AnimateNumber(i.ToString(), tickSound));
        }

        // 3. ¡GO!
        yield return StartCoroutine(AnimateNumber("GO!", goSound));

        // 4. Reanudar juego
        if (countdownPanel != null) countdownPanel.SetActive(false);
        
        // FUERZA BRUTA: Asegurar que el tiempo sea 1
        Time.timeScale = 1f;
        Debug.Log("StartCountdown: ¡TIEMPO REANUDADO A 1!");

        if (player != null) player.enabled = true;
        
        // SEGURIDAD: Re-activar el tiempo de forma robusta
        if (gameTimer == null) gameTimer = GameTimer.Instance;
        if (gameTimer == null) gameTimer = Object.FindFirstObjectByType<GameTimer>();

        if (gameTimer != null)
        {
            gameTimer.StartTimer();
        }
        else
        {
            // ÚLTIMA ALTERNATIVA: Buscar por nombre si todo falla
            GameObject timerObj = GameObject.Find("GameTimer") ?? GameObject.Find("GameManager");
            if (timerObj != null)
            {
                gameTimer = timerObj.GetComponent<GameTimer>();
                if (gameTimer != null) gameTimer.StartTimer();
            }
        }
    }

    private IEnumerator AnimateNumber(string text, AudioClip clip)
    {
        if (countdownText == null) yield break;

        countdownText.text = text;
        countdownText.transform.localScale = Vector3.zero;
        
        // Reproducir sonido (usando PlayOneShot para que no se corten entre sí si son cortos)
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);

        float timer = 0f;
        float duration = 0.7f; // Duración de cada número en pantalla

        while (timer < duration)
        {
            // Importante: usar unscaledDeltaTime porque timeScale es 0
            timer += Time.unscaledDeltaTime;
            float t = timer / duration;
            
            // Efecto de Zoom In con un poco de bounce al final
            // Curva de escala personalizada
            float scale;
            if (t < 0.5f)
            {
                // De 0 a 1.3 (overshoot) en la primera mitad
                scale = Mathf.Lerp(0f, 1.3f, t * 2f);
            }
            else
            {
                // De 1.3 a 1.0 en la segunda mitad
                scale = Mathf.Lerp(1.3f, 1.0f, (t - 0.5f) * 2f);
            }
            
            countdownText.transform.localScale = Vector3.one * scale;
            
            yield return null;
        }

        // Pequeña pausa opcional entre números
        yield return new WaitForSecondsRealtime(0.1f);
    }
}
