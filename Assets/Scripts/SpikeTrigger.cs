using UnityEngine;
using PlayerSystem; // Importante para reconocer el namespace

public class SpikeTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Referencia explícita al namespace para evitar errores de definición
            PlayerSystem.PlayerController playerScript = other.GetComponentInParent<PlayerSystem.PlayerController>();

            if (playerScript != null)
            {
                playerScript.Die();
            }
        }
    }
}