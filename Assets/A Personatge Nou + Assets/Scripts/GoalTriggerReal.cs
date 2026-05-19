using UnityEngine;

public class GoalTriggerReal : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag(playerTag)) return;

        hasTriggered = true;

        // 1. Detener el movimiento del jugador
        StopPlayerMovement(other.gameObject);

        // 2. Detener el cronómetro
        if (GameTimer.Instance != null)
            GameTimer.Instance.StopTimer();

        // 3. Mostrar la interfaz de nivel completado
        if (LevelCompleteUI.Instance != null)
            LevelCompleteUI.Instance.ShowLevelComplete();
    }

    private void StopPlayerMovement(GameObject player)
    {
        // Si usas CharacterController (común en 3D)
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        // Si usas Rigidbody (física)
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // Evita que siga cayendo o moviéndose por inercia
        }

      
    }
}