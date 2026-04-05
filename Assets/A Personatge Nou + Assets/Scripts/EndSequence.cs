using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndSequence : MonoBehaviour
{
    public static EndSequence Instance;

    [Header("Refs")]
    [SerializeField] private Image fadePanel;
    [SerializeField] private Transform player;
    [SerializeField] private CelestePlayer playerController;

    [Header("Tiempos")]
    [SerializeField] private float fadeDuration   = 1f;
    [SerializeField] private float zoomDuration   = 2f;
    [SerializeField] private float zoomTargetSize = 3f;

    private Camera cam;
    private float originalSize;

    private void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
        originalSize = cam.orthographicSize;
    }

    public void StartEndSequence()
    {
        StartCoroutine(EndCoroutine());
    }

    private IEnumerator EndCoroutine()
    {
        GameTimer.Instance.StopTimer();

        playerController.enabled = false;

        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        Vector3 pos = player.position;
        pos.z = transform.position.z;
        transform.position = pos;

        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));
        yield return StartCoroutine(ZoomIn());
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
}