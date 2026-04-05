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

    void Awake()
    {
        if (textoBoton == null) textoBoton = GetComponentInChildren<TextMeshProUGUI>();
        colorOriginal = textoBoton.color;
    }

    // Se activa al pasar el mouse por encima
    public void OnPointerEnter(PointerEventData eventData)
    {
        rutinaParpadeo = StartCoroutine(Parpadear());
    }

    // Se activa al quitar el mouse
    public void OnPointerExit(PointerEventData eventData)
    {
        StopCoroutine(rutinaParpadeo);
        textoBoton.color = colorOriginal; // Volvemos al color base
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