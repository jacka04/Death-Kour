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