using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    [SerializeField] private float     speed    = 15f;
    [SerializeField] private float     lifetime =  4f;
    [SerializeField] private LayerMask destroyLayers; // selecciona Ground, Wall, etc. en el Inspector

    private Vector3 direction;

    public void Launch(Vector3 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleImpact(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleImpact(other.gameObject);
    }

    private void HandleImpact(GameObject obj)
    {
        // Mata al jugador
        CelestePlayer player = obj.GetComponent<CelestePlayer>();
        if (player != null)
        {
            player.Die();
            Destroy(gameObject);
            return;
        }

        // Destruye la bala si toca cualquier capa seleccionada en destroyLayers
        if (((1 << obj.layer) & destroyLayers) != 0)
        {
            Destroy(gameObject);
        }
    }
}