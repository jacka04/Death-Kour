using System.Collections;
using UnityEngine;
using PlayerSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class FallingPlatform : MonoBehaviour
{
    [Header("Tiempos (Segundos)")]
    [SerializeField] private float _shakeDuration = 0.5f;
    [SerializeField] private float _fallDurationBeforeDestroy = 0.3f;

    [Header("Efecto")]
    [SerializeField] private float _shakeAmount = 0.05f;

    private Rigidbody2D _rb;
    private Vector3 _startPos;
    private bool _isFalling = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.useFullKinematicContacts = true;
        _startPos = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isFalling) return;

        if (collision.gameObject.TryGetComponent<PlayerController3>(out _))
        {
            if (collision.contacts[0].normal.y < -0.5f)
            {
                StartCoroutine(ShakeAndFallSequence());
            }
        }
    }

    private IEnumerator ShakeAndFallSequence()
    {
        _isFalling = true;

        float elapsed = 0;
        while (elapsed < _shakeDuration)
        {
            transform.position = _startPos + (Vector3)(Random.insideUnitCircle * _shakeAmount);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = _startPos;
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        yield return new WaitForSeconds(_fallDurationBeforeDestroy);

        Destroy(gameObject);
    }
}