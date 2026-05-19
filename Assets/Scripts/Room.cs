using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject VirtualCam;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            if (VirtualCam != null)
            {
                VirtualCam.SetActive(true);
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