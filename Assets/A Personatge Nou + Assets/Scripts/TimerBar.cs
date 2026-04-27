using UnityEngine;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image fillImage;

    [Header("Colores")]
    [SerializeField] private Color colorFull    = new Color(0.4f, 0.8f, 1f);   
    [SerializeField] private Color colorWarning = new Color(1f, 0.6f, 0.1f);   
    [SerializeField] private Color colorCritical = Color.red;

    [Header("Umbrales (0-1)")]
    [SerializeField] private float warningThreshold  = 0.4f;
    [SerializeField] private float criticalThreshold = 0.15f;

    [Header("Pulso crítico")]
    [SerializeField] private float pulseSpeed = 4f;
    [SerializeField] private float pulseMin   = 0.4f;

    private void Update()
    {
        if (GameTimer.Instance == null || fillImage == null) return;

        float ratio = GameTimer.Instance.TimeLeft / GameTimer.Instance.TotalTime;
        ratio = Mathf.Clamp01(ratio);

        
        fillImage.fillAmount = ratio;

        
        Color targetColor;
        if (ratio <= criticalThreshold)
            targetColor = colorCritical;
        else if (ratio <= warningThreshold)
            targetColor = Color.Lerp(colorCritical, colorWarning,
                          (ratio - criticalThreshold) / (warningThreshold - criticalThreshold));
        else
            targetColor = Color.Lerp(colorWarning, colorFull,
                          (ratio - warningThreshold) / (1f - warningThreshold));

        
        if (ratio <= criticalThreshold)
        {
            float pulse = Mathf.Lerp(pulseMin, 1f, (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f);
            targetColor.a = pulse;
        }

        fillImage.color = targetColor;
    }
}