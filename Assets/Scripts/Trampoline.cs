using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public float jumpForce = 20f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerSystem.PlayerController controller = other.GetComponent<PlayerSystem.PlayerController>();

            if (controller != null)
            {
                controller.ApplyExternalImpulse(jumpForce);
            }
        }
    }
}