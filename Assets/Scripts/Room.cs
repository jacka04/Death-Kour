using UnityEngine;

public class trabsu : MonoBehaviour
{

    public GameObject VirtualCam;
    public Transform spawnPoint; // PUNTO MANUAL DE APARICIÓN

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            VirtualCam.SetActive(true);

            // Si hay un spawnPoint asignado, usamos su posición, si no, la del trigger
            Vector2 pos = spawnPoint != null ? (Vector2)spawnPoint.position : (Vector2)other.transform.position;
            other.GetComponent<PlayerController>().ActualizarCheckpoint(pos);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            VirtualCam.SetActive(false);
        }
    }
}