using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class BotonIntermitente : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Configuración de Colores")]
    public TextMeshProUGUI textoBoton;
    public Color color1 = Color.white;
    public Color color2 = Color.yellow;
    public float velocidad = 0.2f;

    [Header("Zoom")]
    public float escalaHover = 1.15f;
    public float velocidadZoom = 0.1f;

    [Header("Sonidos")]
    public AudioClip sonidoHover;
    public AudioClip sonidoClick;

    private Color colorOriginal;
    private Coroutine rutinaParpadeo;
    private Vector3 escalaOriginal;
    private AudioSource audioSource;

    void Awake()
    {
        if (textoBoton == null) textoBoton = GetComponentInChildren<TextMeshProUGUI>();
        colorOriginal = textoBoton.color;
        escalaOriginal = transform.localScale;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rutinaParpadeo = StartCoroutine(Parpadear());
        StartCoroutine(Zoom(escalaOriginal * escalaHover));

        if (sonidoHover != null)
            audioSource.PlayOneShot(sonidoHover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopCoroutine(rutinaParpadeo);
        textoBoton.color = colorOriginal;
        StartCoroutine(Zoom(escalaOriginal));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (sonidoClick != null)
            audioSource.PlayOneShot(sonidoClick);
    }

    IEnumerator Zoom(Vector3 targetScale)
    {
        Vector3 startScale = transform.localScale;
        float timer = 0f;

        while (timer < velocidadZoom)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / velocidadZoom;
            t = t * t * (3f - 2f * t);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transform.localScale = targetScale;
    }

    IEnumerator Parpadear()
    {
        while (true)
        {
            textoBoton.color = (textoBoton.color == color1) ? color2 : color1;
            yield return new WaitForSecondsRealtime(velocidad);
        }
    }
}