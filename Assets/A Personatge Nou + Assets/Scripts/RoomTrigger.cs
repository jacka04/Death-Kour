using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public Vector3 cameraPosition;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        var col = GetComponent<BoxCollider>();
        if (col != null)
            Gizmos.DrawWireCube(
                transform.position + col.center,
                col.size
            );
    }
}