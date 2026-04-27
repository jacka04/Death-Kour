using System.Collections;
using UnityEngine;

/// <summary>
/// SpringJump — colócalo en el GameObject del muelle.
/// El muelle detecta al jugador por trigger y le aplica un impulso vertical.
/// Requiere un Collider con "Is Trigger" activado.
/// </summary>
[RequireComponent(typeof(Collider))]
public class SpringJump : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // INSPECTOR
    // -------------------------------------------------------------------------
    [Header("Fuerza")]
    [Tooltip("Velocidad vertical que se aplica al jugador al pisar el muelle.")]
    [SerializeField] private float launchSpeed = 28f;

    [Tooltip("Si true, también resetea el dash del jugador al saltar.")]
    [SerializeField] private bool refillDash = true;

    [Tooltip("Velocidad horizontal que conserva el jugador (1 = 100%). Bájalo para frenar al salir.")]
    [Range(0f, 1.5f)]
    [SerializeField] private float horizontalSpeedMult = 1f;

    [Header("Cooldown")]
    [Tooltip("Segundos hasta que el muelle puede activarse de nuevo.")]
    [SerializeField] private float cooldown = 0.5f;

    [Header("Animación")]
    [SerializeField] private Animator springAnim;

    [Header("Sonido")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip   springSound;

    // -------------------------------------------------------------------------
    // RUNTIME
    // -------------------------------------------------------------------------
    private bool isReady = true;

    // -------------------------------------------------------------------------
    // TRIGGER
    // -------------------------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (!isReady) return;

        CelestePlayer player = other.GetComponent<CelestePlayer>();
        if (player == null) return;

        // Solo activar si el jugador viene desde arriba (cae sobre el muelle)
        // Puedes eliminar esta condición si quieres que se active lateralmente también
        if (player.Speed.y > 2f) return;   // Si sube rápido, ignora

        Launch(player);
    }

    // -------------------------------------------------------------------------
    // LANZAR
    // -------------------------------------------------------------------------
    private void Launch(CelestePlayer player)
    {
        isReady = false;

        // Acceder a los campos internos del jugador vía el método público
        player.SpringLaunch(launchSpeed, horizontalSpeedMult, refillDash);

        // Animación
        springAnim?.SetTrigger("Bounce");

        // Sonido
        if (audioSource != null && springSound != null)
            audioSource.PlayOneShot(springSound);

        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(cooldown);
        isReady = true;
        springAnim?.SetTrigger("Reset");
    }

    // -------------------------------------------------------------------------
    // GIZMO (editor)
    // -------------------------------------------------------------------------
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.5f, 0.3f);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * (launchSpeed * 0.1f));
    }
}