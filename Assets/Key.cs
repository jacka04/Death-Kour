using UnityEngine;

/// <summary>
/// Llave levitando en el mapa.
/// 
/// Estados:
///   1. IDLE     → flota en su sitio
///   2. FOLLOWING → sigue al jugador visiblemente (efecto imán)
///   3. COLLECTED → llegó al jugador, avisa a la puerta y desaparece
/// </summary>
public class Key : MonoBehaviour
{
    [Header("Levitación idle")]
    [SerializeField] private float floatAmplitude = 0.3f;
    [SerializeField] private float floatSpeed     = 2f;

    [Header("Efecto imán (siguiendo al jugador)")]
    [SerializeField] private float magnetSpeed    = 6f;
    [Tooltip("Offset respecto al jugador donde orbita la llave mientras lo sigue")]
    [SerializeField] private Vector3 followOffset = new Vector3(0.6f, 0.4f, 0f);
    [Tooltip("Distancia para darse por recogida definitivamente")]
    [SerializeField] private float collectDist    = 0.3f;

    [Header("Puerta que abre")]
    [SerializeField] private Door targetDoor;
    [Tooltip("Distancia a la puerta para que se abra automáticamente")]
    [SerializeField] private float doorOpenDistance = 1.5f;

    [Header("Efectos")]
    [SerializeField] private Color glowColor = new Color(1f, 0.9f, 0.4f, 0.5f); // Amarillo suave
    public float glowSize = 0.8f;
    public float glowPulseIntensity = 0.1f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip idleSound;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] [Range(0f, 1f)] private float idleVolume = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float pickupVolume = 1f;
    [SerializeField] [Range(0f, 1f)] private float spatialBlend = 1f; // 0 = 2D, 1 = 3D
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 10f;

    private SpriteRenderer glowSprite;
    private AudioSource idleAudioSource;

    // ─────────────────────────────────────────────────────────────────────────

    private enum KeyState { Idle, Following, Collected }
    private KeyState state    = KeyState.Idle;
    private Vector3  startPos;
    private Transform playerTr;

    private void Start()
    {
        startPos = transform.position;

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

        // Configurar y reproducir sonido 3D de idle
        if (idleSound != null)
        {
            idleAudioSource = gameObject.AddComponent<AudioSource>();
            idleAudioSource.clip = idleSound;
            idleAudioSource.loop = true;
            idleAudioSource.volume = idleVolume;
            idleAudioSource.spatialBlend = spatialBlend;
            idleAudioSource.minDistance = minDistance;
            idleAudioSource.maxDistance = maxDistance;
            idleAudioSource.rolloffMode = AudioRolloffMode.Linear;
            idleAudioSource.Play();
        }
    }

    private void Update()
    {
        switch (state)
        {
            case KeyState.Idle:
                // Flota suavemente en su sitio
                float y = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
                transform.position = new Vector3(startPos.x, y, 0f);
                break;

            case KeyState.Following:
                // Sigue al jugador hacia el offset, visible todo el tiempo
                Vector3 target = playerTr.position + followOffset;
                target.z = 0f;

                // Pequeña oscilación mientras sigue para que se vea vivo
                target.y += Mathf.Sin(Time.time * floatSpeed * 1.5f) * 0.1f;

                transform.position = Vector3.MoveTowards(
                    transform.position, target, magnetSpeed * Time.deltaTime);

                // NUEVA LÓGICA: Solo se abre si la LLAVE está cerca de la PUERTA
                if (targetDoor != null)
                {
                    float distToDoor = Vector3.Distance(transform.position, targetDoor.transform.position);
                    if (distToDoor <= doorOpenDistance)
                    {
                        Collect();
                    }
                }
                break;

            case KeyState.Collected:
                // Nada, ya está desactivada
                break;
        }

        // Animación del pulso del brillo
        if (glowSprite != null && state != KeyState.Collected)
        {
            float pulse = glowSize + glowPulseIntensity * Mathf.Sin(Time.time * floatSpeed * 2f);
            glowSprite.transform.localScale = Vector3.one * pulse;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (state != KeyState.Idle) return;

        CelestePlayer player = other.GetComponent<CelestePlayer>();
        if (player != null)
        {
            playerTr = other.transform;
            state    = KeyState.Following;

            // Detener el sonido de idle 3D
            if (idleAudioSource != null) idleAudioSource.Stop();

            // Reproducir sonido de recogida
            if (pickupSound != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, pickupVolume);

            // Desactiva el collider para que no vuelva a triggear
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }
    }

    private void Collect()
    {
        state = KeyState.Collected;
        gameObject.SetActive(false);

        if (targetDoor != null)
            targetDoor.Open();
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