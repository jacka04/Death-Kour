using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CelestePlayer player = other.GetComponentInParent<CelestePlayer>();
            if (player != null)
            {
                player.ActualizarCheckpoint(transform.position);
                Debug.Log("Checkpoint alcanzado!");
            }
        }
    }
}