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

    private bool collected = false;

    private void Update()
    {
        if (collected) return;

        // Detecta si el player está dentro del radio, sin necesitar Rigidbody2D
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);

        if (hit != null)
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

    // Dibuja el radio de detección en el editor para que puedas ajustarlo visualmente
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}