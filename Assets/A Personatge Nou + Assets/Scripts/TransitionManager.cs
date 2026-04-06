using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    [Header("Referencias")]
    [SerializeField] private RectTransform swipePanel;

    [Header("Ajustes")]
    [SerializeField] private float duration = 0.4f;
    [SerializeField] private AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private float screenWidth;

    void Awake()
{
    if (Instance != null) { Destroy(gameObject); return; }
    Instance = this;
    DontDestroyOnLoad(gameObject);

    screenWidth = Screen.width;
    // ya no necesitas posicionarlo aquí
}   

    // ─── Para cambiar de CANVAS / panel en el mismo menú ───────────────────
    public void TransitionCanvas(System.Action onMidpoint)
    {
        StartCoroutine(DoCanvasTransition(onMidpoint));
    }

    // ─── Para cambiar de ESCENA ─────────────────────────────────────────────
    public void TransitionScene(string sceneName)
    {
        StartCoroutine(DoSceneTransition(sceneName));
    }

    // ───────────────────────────────────────────────────────────────────────
    private IEnumerator DoCanvasTransition(System.Action onMidpoint)
    {
        yield return StartCoroutine(SlideIn());
        onMidpoint?.Invoke();          // aquí cambias el canvas activo
        yield return new WaitForSeconds(0.05f);
        yield return StartCoroutine(SlideOut());
    }

    private IEnumerator DoSceneTransition(string sceneName)
    {
        yield return StartCoroutine(SlideIn());

        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        load.allowSceneActivation = false;

        while (load.progress < 0.9f)
            yield return null;

        load.allowSceneActivation = true;
        yield return null; // espera un frame a que la escena esté lista

        yield return StartCoroutine(SlideOut());
    }

    // ─── Swipe LEFT: entra desde la derecha, sale hacia la izquierda ────────
   private IEnumerator SlideIn()
{
    swipePanel.gameObject.SetActive(true); // se activa justo antes de entrar
    Vector2 start = new Vector2(screenWidth, 0);
    Vector2 end   = new Vector2(0, 0);
    yield return Slide(start, end);
}

private IEnumerator SlideOut()
{
    Vector2 start = new Vector2(0, 0);
    Vector2 end   = new Vector2(-screenWidth, 0);
    yield return Slide(start, end);

    swipePanel.anchoredPosition = new Vector2(screenWidth, 0);
    swipePanel.gameObject.SetActive(false); // se desactiva al salir de pantalla
}

    private IEnumerator Slide(Vector2 from, Vector2 to)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = curve.Evaluate(Mathf.Clamp01(elapsed / duration));
            swipePanel.anchoredPosition = Vector2.LerpUnclamped(from, to, t);
            yield return null;
        }
        swipePanel.anchoredPosition = to;
    }
}