using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    [Header("Refs")]
    [SerializeField] private RectTransform swipePanel;

    [Header("Configuración")]
    [SerializeField] private float swipeDuration = 0.4f;

    // Posiciones fuera de pantalla (diagonal)
    private Vector2 offScreenLeft  = new Vector2(-1600f, -900f);
    private Vector2 offScreenRight = new Vector2( 1600f,  900f);
    private Vector2 center         = new Vector2(0f, 0f);

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Empieza fuera de pantalla
        swipePanel.anchoredPosition = offScreenLeft;
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(TransitionCoroutine(sceneName));
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(TransitionCoroutine(sceneIndex));
    }

    private IEnumerator TransitionCoroutine(object scene)
    {
        // Entra desde la izquierda hasta el centro
        yield return StartCoroutine(Swipe(offScreenLeft, center));

        // Cargar escena
        if (scene is string)
            SceneManager.LoadScene((string)scene);
        else
            SceneManager.LoadScene((int)scene);

        yield return null;

        // Sale hacia la derecha
        yield return StartCoroutine(Swipe(center, offScreenRight));

        // Resetear posición
        swipePanel.anchoredPosition = offScreenLeft;
    }

    private IEnumerator Swipe(Vector2 from, Vector2 to)
    {
        float timer = 0f;

        while (timer < swipeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / swipeDuration;
            // Ease in out
            t = t * t * (3f - 2f * t);
            swipePanel.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }

        swipePanel.anchoredPosition = to;
    }
}