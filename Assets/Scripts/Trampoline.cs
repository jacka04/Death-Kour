using UnityEngine;
using PlayerSystem;

public class Trampoline : MonoBehaviour
{
    public float jumpForce = 100f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerSystem.PlayerController controller = other.GetComponentInParent<PlayerSystem.PlayerController>();

            if (controller != null)
            {
                controller.ApplyExternalImpulse(jumpForce);
            }
        }
    }
}