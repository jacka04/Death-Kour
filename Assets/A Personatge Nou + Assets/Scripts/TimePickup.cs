using UnityEngine;

public class TimePickup : MonoBehaviour
{
    [Header("Configuración")]
    public float timeToAdd = 5f;
    public float detectionRadius = 0.6f;

    [Header("Efectos (opcionales)")]
    [SerializeField] private GameObject pickupEffectPrefab;
    [SerializeField] private AudioClip pickupSound;

    [Header("Detección")]
    [SerializeField] private LayerMask playerLayer;

    [Header("Animación")]
    [SerializeField] private float bobAmplitude = 0.15f;
    [SerializeField] private float bobFrequency = 2f;
    [SerializeField] private float rotationSpeed = 90f;

    private bool collected = false;
    private Vector3 originPos;
    private float timeAlive;

    private void Awake()
    {
        originPos = transform.position;
    }

    private void Update()
    {
        if (collected) return;

        // Animación de flote y rotación
        timeAlive += Time.deltaTime;
        transform.position = originPos + Vector3.up * Mathf.Sin(timeAlive * bobFrequency) * bobAmplitude;
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

        if (hits.Length > 0)
            Collect();
    }

    private void Collect()
    {
        collected = true;

        GameTimer.Instance.AddTime(timeToAdd);

        if (pickupEffectPrefab != null)
            Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);

        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}