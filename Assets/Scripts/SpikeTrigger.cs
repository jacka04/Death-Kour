using UnityEngine;
using PlayerSystem; 

public class SpikeTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
            PlayerSystem.PlayerController3 playerScript = other.GetComponentInParent<PlayerSystem.PlayerController3>();

            if (playerScript != null)
            {
                playerScript.Die();
            }
        }
    }
}