using UnityEngine;
using System.Collections;

public class SquishAndStretch : MonoBehaviour
{
    public Transform spriteTransform; 
    public Vector3 originalScale = Vector3.one;

    public Vector2 jumpStretch = new Vector2(0.8f, 1.2f);
    public float jumpDuration = 0.2f;

    public Vector2 landSquash = new Vector2(1.3f, 0.7f);
    public float landDuration = 0.15f;

    private Coroutine currentSquashRoutine;

    void Start()
    {
        if (spriteTransform == null)
        {
            SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();

            if (sr != null)
            {
                spriteTransform = sr.transform;
            }
            else
            {
                enabled = false;
                return;
            }
        }
        
        originalScale = spriteTransform.localScale;
    }

    public void PlayJumpStretch()
    {
        if (spriteTransform == null) return;
        StopCurrentAnimation();
        currentSquashRoutine = StartCoroutine(ProcessSquashStretch(jumpStretch, jumpDuration));
    }

    public void PlayLandSquash()
    {
        if (spriteTransform == null) return;
        StopCurrentAnimation();
        currentSquashRoutine = StartCoroutine(ProcessSquashStretch(landSquash, landDuration));
    }

    private void StopCurrentAnimation()
    {
        if (currentSquashRoutine != null)
        {
            StopCoroutine(currentSquashRoutine);
        }
        if (spriteTransform != null)
        {
            spriteTransform.localScale = originalScale;
        }
    }

    private IEnumerator ProcessSquashStretch(Vector2 targetScale2D, float duration)
    {
        float elapsed = 0f;
        Vector3 targetScale = new Vector3(targetScale2D.x, targetScale2D.y, originalScale.z);
        
        float firstHalfDuration = duration / 2f;
        while (elapsed < firstHalfDuration)
        {
            spriteTransform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / firstHalfDuration);
            elapsed += Time.deltaTime;
            yield return null; 
        }
        spriteTransform.localScale = targetScale;

        elapsed = 0f;
        float secondHalfDuration = duration / 2f;
        while (elapsed < secondHalfDuration)
        {
            spriteTransform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / secondHalfDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        spriteTransform.localScale = originalScale;
        currentSquashRoutine = null;
    }
}