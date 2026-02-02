using UnityEngine;
using PlayerSystem;

public class Room : MonoBehaviour
{
    public GameObject VirtualCam;
    public Transform spawnPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            if (VirtualCam != null)
            {
                VirtualCam.SetActive(true);
            }

            Vector2 pos = spawnPoint != null ? (Vector2)spawnPoint.position : (Vector2)other.transform.position;

            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ActualizarCheckpoint(pos);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            if (VirtualCam != null)
            {
                VirtualCam.SetActive(false);
            }
        }
    }
}