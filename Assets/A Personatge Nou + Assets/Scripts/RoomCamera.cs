using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoomCamera : MonoBehaviour
{
  [Header("Fade")]
    [SerializeField] private Image fadePanel;
    [SerializeField] private float fadeDuration = 0.25f;

    [Header("Refs")]
    [SerializeField] private CelestePlayer playerController;
[Header("Room inicial")]
[SerializeField] private RoomTrigger startRoom;

private void Start()
{
    if (startRoom != null)
    {
        Vector3 pos = startRoom.cameraPosition;
        pos.z = transform.position.z;
        transform.position = pos;
        currentRoom = startRoom;
    }
}
    private RoomTrigger currentRoom;
    private bool isTransitioning = false;

    public void OnPlayerEnterRoom(RoomTrigger newRoom)
    {
        if (isTransitioning || newRoom == currentRoom) return;
        StartCoroutine(TransitionCoroutine(newRoom));
    }

    private IEnumerator TransitionCoroutine(RoomTrigger newRoom)
    {
        isTransitioning = true;
        playerController.enabled = false;

        yield return StartCoroutine(Fade(0f, 1f));  

        
        Vector3 pos = newRoom.cameraPosition;
        pos.z = transform.position.z;
        transform.position = pos;
        currentRoom = newRoom;

       yield return StartCoroutine(Fade(1f, 0f));  

        playerController.enabled = true;
        isTransitioning = false;
    }

    private IEnumerator Fade(float from, float to)
    {
        float timer = 0f;
        Color c = fadePanel.color;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, timer / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }
        c.a = to;
        fadePanel.color = c;
    }
}