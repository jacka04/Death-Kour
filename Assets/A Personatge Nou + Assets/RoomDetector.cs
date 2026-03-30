using UnityEngine;

public class RoomDetector : MonoBehaviour
{
    private RoomCamera roomCamera;

    private void Start()
    {
        roomCamera = Camera.main.GetComponent<RoomCamera>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var room = other.GetComponent<RoomTrigger>();
        if (room != null)
            roomCamera.OnPlayerEnterRoom(room);
    }
}