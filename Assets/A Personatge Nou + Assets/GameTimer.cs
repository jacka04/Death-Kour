using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    [Header("Tiempo")]
    [SerializeField] private float totalTime = 20f;
    private float timeLeft;
    private bool isRunning = false;
    private bool hasEnded = false;

    [Header("Refs")]
    [SerializeField] private CelestePlayer playerController;
    [SerializeField] private Transform player;

    [Header("Timer UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Timeout UI")]
    [SerializeField] private GameObject timeoutCanvas;
    [SerializeField] private Image timeoutFadePanel;
    [SerializeField] private GameObject timeoutPanel;

    [Header("Tiempos")]
    [SerializeField] private float fadeDuration   = 1f;
    [SerializeField] private float zoomDuration   = 2f;
    [SerializeField] private float zoomTargetSize = 3f;

    private Camera cam;

    private void Awake()
    {
        Instance = this;
        cam = Camera.main;
        timeLeft = totalTime;
        timeoutCanvas.SetActive(false);
        timeoutPanel.SetActive(false);
    }

    private void Start()
    {
        isRunning = true;
    }

    private void Update()
    {
        if (!isRunning || hasEnded) return;

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            isRunning = false;
            hasEnded = true;
            StartCoroutine(TimeoutSequence());
        }

        UpdateTimerText();
    }

    private void UpdateTimerText()
{
    if (timerText == null) return;

    int seconds      = (int)timeLeft;
    int milliseconds = (int)((timeLeft - seconds) * 100);

    timerText.text = string.Format("{0:00}:{1:00}", seconds, milliseconds);

    if (timeLeft <= 5f)
        timerText.color = Color.red;
    else
        timerText.color = Color.white;
}
    public void StopTimer()
    {
        isRunning = false;
    }

    public float TimeLeft => timeLeft;

  private IEnumerator TimeoutSequence()
{
    playerController.enabled = false;

    timeoutCanvas.SetActive(true);

    yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

    Vector3 pos = player.position;
    pos.z = cam.transform.position.z;
    cam.transform.position = pos;

    yield return StartCoroutine(Fade(1f, 0f, fadeDuration));
    yield return StartCoroutine(ZoomIn());

    // Mostrar panel con animación
    timeoutPanel.SetActive(true);
    yield return StartCoroutine(AnimatePanel());
}

private IEnumerator AnimatePanel()
{
    float timer = 0f;
    float duration = 0.4f;
    RectTransform rect = timeoutPanel.GetComponent<RectTransform>();

    Vector3 targetScale = new Vector3(0.6f, 0.6f, 1f);

    while (timer < duration)
    {
        timer += Time.deltaTime;
        float t = timer / duration;
        t = 1f - Mathf.Pow(1f - t, 3f);
        rect.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
        yield return null;
    }

    rect.localScale = targetScale;
}

    private IEnumerator Fade(float from, float to, float duration)
    {
        float timer = 0f;
        Color c = timeoutFadePanel.color;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, timer / duration);
            timeoutFadePanel.color = c;
            yield return null;
        }
        c.a = to;
        timeoutFadePanel.color = c;
    }

    private IEnumerator ZoomIn()
    {
        float timer = 0f;
        float startSize = cam.orthographicSize;
        while (timer < zoomDuration)
        {
            timer += Time.deltaTime;
            float t = timer / zoomDuration;
            t = t * t * (3f - 2f * t);
            cam.orthographicSize = Mathf.Lerp(startSize, zoomTargetSize, t);
            yield return null;
        }
        cam.orthographicSize = zoomTargetSize;
    }

    public void OnRetryButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}