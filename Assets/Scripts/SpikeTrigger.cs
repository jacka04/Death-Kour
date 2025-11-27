using UnityEngine;

public class SpikeTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerScript = other.GetComponent<PlayerController>();

            if (playerScript != null)
            {
                playerScript.Morir();
            }
        }
    }
}