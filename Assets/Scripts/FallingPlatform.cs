using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private float _fallDelay = 0.5f;
    [SerializeField] private float _destroyDelay = 2f;

    private Rigidbody2D _rb;
    private Vector3 _initialPosition;
    private bool _isFalling = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _initialPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !_isFalling)
        {
            if (collision.contacts[0].normal.y < -0.5f)
            {
                StartCoroutine(Fall());
            }
        }
    }

    private IEnumerator Fall()
    {
        _isFalling = true;

        float elapsed = 0;
        while (elapsed < _fallDelay)
        {
            transform.position = _initialPosition + (Vector3)(Random.insideUnitCircle * 0.05f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = _initialPosition;
        _rb.bodyType = RigidbodyType2D.Dynamic;

        yield return new WaitForSeconds(_destroyDelay);
        Destroy(gameObject);
    }
}