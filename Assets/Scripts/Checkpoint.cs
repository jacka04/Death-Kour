using UnityEngine;
using PlayerSystem;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Usamos GetComponentInParent por si el collider está en un objeto hijo
            PlayerController player = other.GetComponentInParent<PlayerController>();
            
            if (player != null)
            {
                player.ActualizarCheckpoint(transform.position);
                Debug.Log("Checkpoint alcanzado!");
            }
        }
    }
}