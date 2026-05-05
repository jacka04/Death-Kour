using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ReaparecerCheckpoint : MonoBehaviour
{
    private static ReaparecerCheckpoint checkpointActual;
    private bool activado = false;

    private void Awake()
    {
        // Forzar Is Trigger por código, por si se olvidó en el Inspector
        var col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
            col.isTrigger = true;
            Debug.LogWarning($"[Checkpoint] '{gameObject.name}' no tenía Is Trigger. Se activó por código.");
        }

        // Forzar Z = 0 por si el objeto está desplazado
        Vector3 pos = transform.position;
        if (pos.z != 0f)
        {
            pos.z = 0f;
            transform.position = pos;
            Debug.LogWarning($"[Checkpoint] '{gameObject.name}' estaba en Z={pos.z}, corregido a 0.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[Checkpoint] OnTriggerEnter con: {other.gameObject.name} (layer: {LayerMask.LayerToName(other.gameObject.layer)})");

        CelestePlayer player = other.GetComponent<CelestePlayer>()
                            ?? other.GetComponentInParent<CelestePlayer>()
                            ?? other.GetComponentInChildren<CelestePlayer>();

        if (player == null)
        {
            Debug.Log("[Checkpoint] El objeto que entró NO tiene CelestePlayer.");
            return;
        }

        if (activado) return;

        if (checkpointActual != null)
            checkpointActual.activado = false;

        activado = true;
        checkpointActual = this;

        player.ActualizarCheckpoint(transform.position);
        Debug.Log($"[Checkpoint] Activado: {gameObject.name}");
    }
}