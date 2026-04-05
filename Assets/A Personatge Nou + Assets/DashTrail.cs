using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTrail : MonoBehaviour
{
    [Header("Trail")]
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private int trailLength     = 8;
    [SerializeField] private float trailInterval = 0.02f;
    [SerializeField] private float fadeDuration  = 0.15f;
    [SerializeField] private Color trailColor    = new Color(0.4f, 0.9f, 1f, 0.7f);

    private bool isTrailing = false;
    private Coroutine trailCoroutine;

    public void StartTrail()
    {
        if (trailCoroutine != null)
            StopCoroutine(trailCoroutine);
        trailCoroutine = StartCoroutine(TrailCoroutine());
    }

    public void StopTrail()
    {
        isTrailing = false;
    }

    private IEnumerator TrailCoroutine()
    {
        isTrailing = true;

        while (isTrailing)
        {
            SpawnGhost();
            yield return new WaitForSeconds(trailInterval);
        }
    }

    private void SpawnGhost()
    {
        // Crear ghost nuevo cada vez, sin pool
        GameObject go = new GameObject("Ghost");
        go.transform.position = playerSprite.transform.position;
        go.transform.rotation = playerSprite.transform.rotation;
        go.transform.localScale = playerSprite.transform.lossyScale;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite           = playerSprite.sprite;
        sr.flipX            = playerSprite.flipX;
        sr.color            = trailColor;
        sr.sortingLayerName = playerSprite.sortingLayerName;
sr.sortingOrder     = playerSprite.sortingOrder + 2;

        StartCoroutine(FadeAndDestroy(go, sr));
    }

    private IEnumerator FadeAndDestroy(GameObject go, SpriteRenderer sr)
    {
        float timer = 0f;
        Color startColor = trailColor;

        while (timer < fadeDuration)
        {
            if (sr == null) yield break;
            timer += Time.deltaTime;
            float a = Mathf.Lerp(startColor.a, 0f, timer / fadeDuration);
            sr.color = new Color(startColor.r, startColor.g, startColor.b, a);
            yield return null;
        }

        if (go != null)
            Destroy(go);
    }
}