using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private float parallaxFactorX = 0.1f;
    [SerializeField] private float parallaxFactorY = 0f;
    [SerializeField] private float smoothing = 3f; // más alto = más rápido

    private Vector3 startPos;
    private Transform player;
    private Vector3 targetPos;

    private void Start()
    {
        startPos = transform.position;
        player = GameObject.FindWithTag("Player").transform;
        targetPos = startPos;
    }

    private void LateUpdate()
    {
        if (player == null) return;

        targetPos = new Vector3(
            startPos.x + player.position.x * parallaxFactorX,
            startPos.y + player.position.y * parallaxFactorY,
            startPos.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            smoothing * Time.deltaTime
        );
    }
}