using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Coroutine shakeCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    public void Shake(float duration = 0.15f, float magnitude = 0.08f)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float timer = 0f;
        Vector3 startPos = transform.position;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            float currentMagnitude = Mathf.Lerp(magnitude, 0f, progress);

            transform.position = startPos + new Vector3(
                Random.Range(-1f, 1f) * currentMagnitude,
                Random.Range(-1f, 1f) * currentMagnitude,
                0f
            );

            yield return null;
        }

        transform.position = startPos;
    }
}