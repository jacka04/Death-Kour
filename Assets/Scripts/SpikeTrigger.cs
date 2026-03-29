using UnityEngine;

public class SpikeTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CelestePlayer player = other.GetComponentInParent<CelestePlayer>();
            if (player != null)
                player.Die();
        }
    }
}