using UnityEngine;
using PlayerSystem;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ActualizarCheckpoint(transform.position);

                Debug.Log("Checkpoint alcanzado!");
            }
        }
    }
}