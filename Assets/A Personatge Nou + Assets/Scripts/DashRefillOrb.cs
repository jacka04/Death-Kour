using System.Collections;
using UnityEngine;

public class DashRefillOrb : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float respawnTime = 2.5f;
    [SerializeField] private float bobAmplitude = 0.15f;
    [SerializeField] private float bobFrequency = 2f;
    [SerializeField] private float rotationSpeed = 90f;

    [Header("Partículas")]
    [SerializeField] private int burstCount = 12;
    [SerializeField] private float particleSpeed = 4f;
    [SerializeField] private float particleLifetime = 0.5f;
    [SerializeField] private float particleSize = 0.08f;
    [SerializeField] private Color particleColor = new Color(0.4f, 0.8f, 1f);

    [Header("Referencias")]
    [SerializeField] private SpriteRenderer orbSprite;
    [SerializeField] private SpriteRenderer glowSprite;
    [Header("Audio")]
[SerializeField] private AudioClip collectSound;
[SerializeField] [Range(0f, 1f)] private float collectVolume = 1f;

    // ── estado interno ──────────────────────────────────────────
    private Vector3 originPos;
    private bool    isActive = true;
    private float   timeAlive;

    // ── partículas procedurales ─────────────────────────────────
    private struct Particle
    {
        public Vector3 pos;
        public Vector3 vel;
        public float   life;
        public float   maxLife;
    }
    private Particle[]  particles;
    private GameObject  particleRoot;
    private Transform[] particleTransforms;

    // ────────────────────────────────────────────────────────────
    private void Collect()
{
    isActive = false;
    SetVisible(false);
    SpawnBurst();

    if (collectSound != null)
        AudioSource.PlayClipAtPoint(collectSound, transform.position, collectVolume);

    StartCoroutine(RespawnRoutine());
}
    private void Awake()
    {
        originPos = transform.position;

        // Crear pool de partículas procedurales
        particles          = new Particle[burstCount];
        particleTransforms = new Transform[burstCount];
        particleRoot       = new GameObject("OrbParticles");
        particleRoot.transform.SetParent(null);

        for (int i = 0; i < burstCount; i++)
        {
            var go = new GameObject($"p{i}");
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

    private void Update()
    {
        if (isActive)
        {
            // Bob suave arriba/abajo
            timeAlive += Time.deltaTime;
            transform.position = originPos + Vector3.up * Mathf.Sin(timeAlive * bobFrequency) * bobAmplitude;

            // Rotación lenta del sprite (si es un quad 3D, usa transform.Rotate)
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

            // Pulso de escala del glow
            if (glowSprite != null)
            {
                float pulse = 1f + 0.1f * Mathf.Sin(timeAlive * bobFrequency * 2f);
                glowSprite.transform.localScale = Vector3.one * pulse;
            }
        }

        UpdateParticles();
    }

    // ── Detección de colisión ───────────────────────────────────

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        var player = other.GetComponent<CelestePlayer>();
        if (player == null) return;

        if (player.TryRefillDash())
            Collect();
    }

    // ── Recoger orbe ────────────────────────────────────────────

    

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnTime);
        transform.position = originPos;
        timeAlive          = 0f;
        isActive           = true;
        SetVisible(true);
    }

    private void SetVisible(bool visible)
    {
        if (orbSprite  != null) orbSprite.enabled  = visible;
        if (glowSprite != null) glowSprite.enabled = visible;
        GetComponent<Collider>().enabled            = visible;
    }

    // ── Sistema de partículas ────────────────────────────────────

    private void SpawnBurst()
    {
        for (int i = 0; i < burstCount; i++)
        {
            float angle = (360f / burstCount) * i + Random.Range(-15f, 15f);
            float rad   = angle * Mathf.Deg2Rad;
            float speed = particleSpeed * Random.Range(0.6f, 1.4f);

            particles[i] = new Particle
            {
                pos     = transform.position,
                vel     = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * speed,
                life    = 0f,
                maxLife = particleLifetime * Random.Range(0.8f, 1.2f)
            };

            particleTransforms[i].position   = particles[i].pos;
            particleTransforms[i].localScale  = Vector3.one * particleSize;
            particleTransforms[i].gameObject.SetActive(true);

            // Color con variación de tono
            var sr      = particleTransforms[i].GetComponent<SpriteRenderer>();
            sr.color    = particleColor * Random.Range(0.8f, 1.2f);
        }
    }

    private void UpdateParticles()
    {
        for (int i = 0; i < burstCount; i++)
        {
            if (!particleTransforms[i].gameObject.activeSelf) continue;

            ref Particle p = ref particles[i];
            p.life += Time.deltaTime;

            if (p.life >= p.maxLife)
            {
                particleTransforms[i].gameObject.SetActive(false);
                continue;
            }

            float t = p.life / p.maxLife;            // 0 → 1

            // Desaceleración suave
            p.pos += p.vel * (1f - t) * Time.deltaTime;
            particleTransforms[i].position = p.pos;

            // Fade + encogimiento
            float alpha = 1f - t;
            float scale = particleSize * (1f - t * 0.5f);
            particleTransforms[i].localScale = Vector3.one * scale;

            var sr   = particleTransforms[i].GetComponent<SpriteRenderer>();
            var col  = sr.color;
            col.a    = alpha;
            sr.color = col;
        }
    }

    // ── Utilidad: sprite cuadrado en runtime ─────────────────────

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