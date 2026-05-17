using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Referencias")]
    [Tooltip("La imagen que servirá de borde (debe estar detrás de la imagen del nivel).")]
    public Image borderImage;

    [Header("Colores del Borde")]
    public Color normalColor = new Color(0.2f, 0.2f, 0.2f, 1f); // Gris oscuro
    public Color hoverColor = new Color(0f, 0.8f, 1f, 1f); // Azul cian brillante

    [Header("Animación")]
    public float scaleAmount = 1.05f; // Cuánto crece el rectángulo al poner el ratón
    public float transitionSpeed = 12f;

    private Vector3 originalScale;
    private bool isHovered = false;

    private void Awake()
    {
        originalScale = transform.localScale;
        if (borderImage != null)
            borderImage.color = normalColor;
    }

    private void Update()
    {
        // Animación de tamaño suave (Scale)
        Vector3 targetScale = isHovered ? originalScale * scaleAmount : originalScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * transitionSpeed);

        // Animación de color del borde suave
        if (borderImage != null)
        {
            Color targetColor = isHovered ? hoverColor : normalColor;
            borderImage.color = Color.Lerp(borderImage.color, targetColor, Time.unscaledDeltaTime * transitionSpeed);
        }
    }

    // Funciona con el Ratón
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

    // Funciona con Mando / Teclado
    public void OnSelect(BaseEventData eventData)
    {
        isHovered = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isHovered = false;
    }
}
