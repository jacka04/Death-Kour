using UnityEngine;

public class trabsu : MonoBehaviour
{

    public GameObject VirtualCam;

    private void OnTriggerEnter2D(Collider2D other)
    {
      if(other.CompareTag("Player")&& !other.isTrigger)
        {
            VirtualCam.SetActive(true);
        }
    }
     private void OnTriggerExit2D(Collider2D other)
    {
      if(other.CompareTag("Player")&& !other.isTrigger)
        {
            VirtualCam.SetActive(false);
        }
    }
}
