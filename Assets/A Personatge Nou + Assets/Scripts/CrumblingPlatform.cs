using System.Collections;
using UnityEngine;

public class CrumblingPlatform : MonoBehaviour
{
    [Header("Configuración Principal")]
    [Tooltip("Tiempo en segundos que tiembla la plataforma antes de romperse/desaparecer.")]
    public float breakDelay = 1.0f;
    
    [Tooltip("Fuerza del temblor.")]
    public float shakeIntensity = 0.05f;

    [Tooltip("Tiempo que tarda en volver a aparecer. Ponlo en 0 si no quieres que reaparezca.")]
    public float respawnTime = 3.0f;

    [Header("Partículas Procedurales (Polvo)")]
    public int particleCount = 12;
    public Color particleColor = new Color(0.6f, 0.6f, 0.6f);
    public float particleSpeed = 3f;
    public float particleLifetime = 0.6f;
    public float particleSize = 0.15f;

    [Header("Efectos Opcionales")]
    [Tooltip("Sonido al romperse.")]
    public AudioClip breakSound;

    private Vector3 originalPosition;
    private bool isActivated = false;

    // Referencias
    private Collider platformCollider;
    private Renderer[] renderers;

    // ── estado partículas ───────────────────────────────────────
    private struct DustParticle
    {
        public Vector3 pos;
        public Vector3 vel;
        public float   life;
        public float   maxLife;
    }
    private DustParticle[]  particles;
    private GameObject      particleRoot;
    private Transform[]     particleTransforms;

    private void Awake()
    {
        originalPosition = transform.position;
        platformCollider = GetComponent<Collider>();
        renderers = GetComponentsInChildren<Renderer>();

        InitProceduralParticles();
    }

    private void Update()
    {
        UpdateParticles();

        if (!isActivated)
        {
            CheckPlayerOnTop();
        }
    }

    private void CheckPlayerOnTop()
    {
        if (platformCollider == null || !platformCollider.enabled) return;

        // Crea una zona de detección (caja invisible) justo encima de la plataforma
        Vector3 extents = platformCollider.bounds.extents;
        extents.y = 0.2f; // Grosor de la caja de detección
        Vector3 center = platformCollider.bounds.center + Vector3.up * (platformCollider.bounds.extents.y + 0.1f);

        // Detectar si algo toca esta caja
        Collider[] hits = Physics.OverlapBox(center, extents, transform.rotation);
        foreach (var hit in hits)
        {
            if (hit.isTrigger) continue; // Ignorar otros triggers

            CelestePlayer player = hit.GetComponent<CelestePlayer>();
            if (player != null || hit.CompareTag("Player"))
            {
                StartCoroutine(CrumbleRoutine());
                break;
            }
        }
    }

    // Funciona si la plataforma usa colisiones físicas normales
    private void OnCollisionEnter(Collision collision)
    {
        CheckPlayerCollision(collision.gameObject);
    }

    // Funciona si prefieres usar un Trigger (IsTrigger = true) encima de la plataforma
    private void OnTriggerEnter(Collider other)
    {
        CheckPlayerCollision(other.gameObject);
    }

    private void CheckPlayerCollision(GameObject obj)
    {
        if (isActivated) return;

        CelestePlayer player = obj.GetComponent<CelestePlayer>();
        
        if (player != null || obj.CompareTag("Player"))
        {
            StartCoroutine(CrumbleRoutine());
        }
    }

    private IEnumerator CrumbleRoutine()
    {
        isActivated = true;
        float elapsed = 0f;

        // Fase 1: Temblor de la plataforma
        while (elapsed < breakDelay)
        {
            elapsed += Time.deltaTime;
            transform.position = originalPosition + Random.insideUnitSphere * shakeIntensity;
            yield return null;
        }

        transform.position = originalPosition;

        // Fase 2: Romper (desactivar visuales y colisiones)
        BreakPlatform();

        // Fase 3: Reaparecer (si el tiempo es mayor a 0)
        if (respawnTime > 0f)
        {
            yield return new WaitForSeconds(respawnTime);
            RespawnPlatform();
        }
    }

    private void BreakPlatform()
    {
        // Desactivar gráficos
        foreach (var r in renderers)
        {
            if (r != null) r.enabled = false;
        }

        // Desactivar colisiones
        if (platformCollider != null)
            platformCollider.enabled = false;

        // Efecto de polvo procedural
        SpawnDustParticles();

        if (breakSound != null)
            AudioSource.PlayClipAtPoint(breakSound, transform.position);
    }

    private void RespawnPlatform()
    {
        // Activar gráficos
        foreach (var r in renderers)
        {
            if (r != null) r.enabled = true;
        }

        // Activar colisiones
        if (platformCollider != null)
            platformCollider.enabled = true;

        isActivated = false;
    }

    // ── Sistema de partículas procedurales ───────────────────────

    private void InitProceduralParticles()
    {
        particles          = new DustParticle[particleCount];
        particleTransforms = new Transform[particleCount];
        
        // Creamos un nodo hijo para contener las partículas para que no ensucien la jerarquía
        particleRoot = new GameObject("PlatformDust");
        particleRoot.transform.SetParent(transform);

        for (int i = 0; i < particleCount; i++)
        {
            var go = new GameObject($"dust_{i}");
            go.transform.SetParent(particleRoot.transform);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite        = CreateSquareSprite();
            sr.color         = particleColor;
            sr.sortingOrder  = 5;
            go.transform.localScale = Vector3.one * particleSize;
            go.SetActive(false);

            particleTransforms[i] = go.transform;
        }
    }

    private void SpawnDustParticles()
    {
        if (particles == null) return;

        for (int i = 0; i < particleCount; i++)
        {
            // Generar una dirección aleatoria de explosión hacia afuera
            Vector3 spawnOffset = Random.insideUnitSphere * 0.3f;
            float angle = Random.Range(0f, 360f);
            float rad   = angle * Mathf.Deg2Rad;
            float speed = particleSpeed * Random.Range(0.6f, 1.4f);

            particles[i] = new DustParticle
            {
                pos     = transform.position + spawnOffset,
                vel     = new Vector3(Mathf.Cos(rad), Random.Range(0.2f, 1f), Mathf.Sin(rad)).normalized * speed,
                life    = 0f,
                maxLife = particleLifetime * Random.Range(0.8f, 1.2f)
            };

            particleTransforms[i].position   = particles[i].pos;
            particleTransforms[i].localScale = Vector3.one * particleSize;
            particleTransforms[i].gameObject.SetActive(true);

            // Variación leve del color original
            var sr   = particleTransforms[i].GetComponent<SpriteRenderer>();
            sr.color = particleColor * Random.Range(0.8f, 1.2f);
        }
    }

    private void UpdateParticles()
    {
        if (particles == null) return;

        for (int i = 0; i < particleCount; i++)
        {
            if (!particleTransforms[i].gameObject.activeSelf) continue;

            ref DustParticle p = ref particles[i];
            p.life += Time.deltaTime;

            if (p.life >= p.maxLife)
            {
                particleTransforms[i].gameObject.SetActive(false);
                continue;
            }

            float t = p.life / p.maxLife; // 0 → 1

            // Desaceleración suave (resistencia al aire)
            p.pos += p.vel * (1f - t) * Time.deltaTime;
            
            // Gravedad leve para que el polvo caiga un poco
            p.pos += Vector3.down * 1.5f * t * Time.deltaTime;
            
            particleTransforms[i].position = p.pos;

            // Fade (desvanecimiento) y encogimiento
            float alpha = 1f - t;
            float scale = particleSize * (1f - t * 0.5f);
            particleTransforms[i].localScale = Vector3.one * scale;

            var sr   = particleTransforms[i].GetComponent<SpriteRenderer>();
            var col  = sr.color;
            col.a    = alpha;
            sr.color = col;
        }
    }

    // Crea un sprite cuadrado genérico desde código
    private Sprite CreateSquareSprite()
    {
        var tex = new Texture2D(4, 4);
        var pixels = new Color[16];
        for (int i = 0; i < 16; i++) pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 4, 4), Vector2.one * 0.5f, 16f);
    }
}
