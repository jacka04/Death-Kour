using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Efectos de Brillo")]
    [SerializeField] private Color glowColor = new Color(0.4f, 0.8f, 1f, 0.5f); // Cyan suave
    public float glowSize = 1.2f;
    public float glowPulseIntensity = 0.2f;
    public float pulseSpeed = 2f;
    private SpriteRenderer glowSprite;

    private void Start()
    {
        // Crear aura brillante (bloom) automáticamente
        if (glowSprite == null)
        {
            GameObject glowObj = new GameObject("ProceduralGlow");
            glowObj.transform.SetParent(transform);
            glowObj.transform.localPosition = Vector3.zero;

            glowSprite = glowObj.AddComponent<SpriteRenderer>();
            glowSprite.sprite = CreateRadialGradientSprite(128);
            glowSprite.color = glowColor;
            glowSprite.sortingOrder = 4;
        }
    }

    private void Update()
    {
        // Animación del pulso del brillo
        if (glowSprite != null)
        {
            float pulse = glowSize + glowPulseIntensity * Mathf.Sin(Time.time * pulseSpeed);
            glowSprite.transform.localScale = Vector3.one * pulse;
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player"))
        {
            CelestePlayer player = other.GetComponentInParent<CelestePlayer>();

            if (player != null)
            {
                player.ActualizarCheckpoint(transform.position);
                Debug.Log("Checkpoint detectado mediante Collider 3D en: " + transform.position);
            }
        }
    }

    private Sprite CreateRadialGradientSprite(int size)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var pixels = new Color[size * size];
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                float t = 1f - Mathf.Clamp01(dist / radius);
                float alpha = t * t * t; 
                pixels[y * size + x] = new Color(1, 1, 1, alpha);
            }
        }
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size / 4f);
    }
}