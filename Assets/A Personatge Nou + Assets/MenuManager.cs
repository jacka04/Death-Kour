using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject canvasMenu;
    [SerializeField] private GameObject canvasNiveles;
    [SerializeField] private GameObject canvasOpciones;

    [Header("Fade")]
    [SerializeField] private Image fadePanel;
    [SerializeField] private float fadeDuration = 0.2f;

    // Botones
    public void OnClickJugar()
    {
        StartCoroutine(CambiarCanvas(canvasMenu, canvasNiveles));
    }

    public void OnClickOpciones()
    {
        StartCoroutine(CambiarCanvas(canvasMenu, canvasOpciones));
    }

    public void OnClickVolverDesdeNiveles()
    {
        StartCoroutine(CambiarCanvas(canvasNiveles, canvasMenu));
    }

    public void OnClickVolverDesdeOpciones()
    {
        StartCoroutine(CambiarCanvas(canvasOpciones, canvasMenu));
    }

    public void OnClickSalir()
    {
        Application.Quit();
    }

    public void OnClickNivel(int sceneIndex)
    {
        SceneTransition.Instance.LoadScene(sceneIndex);
    }

    // Transición entre canvas
    private IEnumerator CambiarCanvas(GameObject desde, GameObject hasta)
    {
        // Fade a negro
        yield return StartCoroutine(Fade(0f, 1f));

        desde.SetActive(false);
        hasta.SetActive(true);

        // Fade de vuelta
        yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float from, float to)
    {
        float timer = 0f;
        Color c = fadePanel.color;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, timer / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }
        c.a = to;
        fadePanel.color = c;
    }
}