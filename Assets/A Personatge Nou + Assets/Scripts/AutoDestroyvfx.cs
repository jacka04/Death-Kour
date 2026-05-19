using UnityEngine;

/// <summary>
/// Añade este componente a cualquier GameObject con ParticleSystem
/// para que se destruya automáticamente cuando el efecto termine.
/// Lo usa TurretBullet para los VFX de impacto.
/// </summary>
public class AutoDestroyVFX : MonoBehaviour
{
    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        if (ps == null)
        {
            // Busca en hijos
            ps = GetComponentInChildren<ParticleSystem>();
        }
    }

    private void Update()
    {
        if (ps == null)
        {
            Destroy(gameObject);
            return;
        }

        // Destruye cuando el sistema dejó de emitir y no quedan partículas vivas
        if (!ps.IsAlive(true))
            Destroy(gameObject);
    }
}