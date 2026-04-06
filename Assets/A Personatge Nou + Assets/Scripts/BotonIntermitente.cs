using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class BotonIntermitente : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configuración de Colores")]
    public TextMeshProUGUI textoBoton;
    public Color color1 = Color.white;
    public Color color2 = Color.yellow;
    public float velocidad = 0.2f;

    private Color colorOriginal;
    private Coroutine rutinaParpadeo;
    private Vector3 escalaOriginal;

    [Header("Zoom")]
    public float escalaHover = 1.15f;
    public float velocidadZoom = 0.1f;

    void Awake()
    {
        if (textoBoton == null) textoBoton = GetComponentInChildren<TextMeshProUGUI>();
        colorOriginal = textoBoton.color;
        escalaOriginal = transform.localScale;
    }

    // Se activa al pasar el mouse por encima
   public void OnPointerEnter(PointerEventData eventData)
    {
        rutinaParpadeo = StartCoroutine(Parpadear());
        StartCoroutine(Zoom(escalaOriginal * escalaHover));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopCoroutine(rutinaParpadeo);
        textoBoton.color = colorOriginal;
        StartCoroutine(Zoom(escalaOriginal));
    }

    IEnumerator Zoom(Vector3 targetScale)
    {
        Vector3 startScale = transform.localScale;
        float timer = 0f;

        while (timer < velocidadZoom)
        {
            timer += Time.deltaTime;
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
            // Cambia entre color1 y color2
            textoBoton.color = (textoBoton.color == color1) ? color2 : color1;
            yield return new WaitForSeconds(velocidad);
        }
    }
}