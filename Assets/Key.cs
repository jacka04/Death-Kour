using UnityEngine;

/// <summary>
/// Llave levitando en el mapa.
/// 
/// Estados:
///   1. IDLE     → flota en su sitio
///   2. FOLLOWING → sigue al jugador visiblemente (efecto imán)
///   3. COLLECTED → llegó al jugador, avisa a la puerta y desaparece
/// </summary>
public class Key : MonoBehaviour
{
    [Header("Levitación idle")]
    [SerializeField] private float floatAmplitude = 0.3f;
    [SerializeField] private float floatSpeed     = 2f;

    [Header("Efecto imán (siguiendo al jugador)")]
    [SerializeField] private float magnetSpeed    = 6f;
    [Tooltip("Offset respecto al jugador donde orbita la llave mientras lo sigue")]
    [SerializeField] private Vector3 followOffset = new Vector3(0.6f, 0.4f, 0f);
    [Tooltip("Distancia para darse por recogida definitivamente")]
    [SerializeField] private float collectDist    = 0.3f;

    [Header("Puerta que abre")]
    [SerializeField] private Door targetDoor;

    // ─────────────────────────────────────────────────────────────────────────

    private enum KeyState { Idle, Following, Collected }
    private KeyState state    = KeyState.Idle;
    private Vector3  startPos;
    private Transform playerTr;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        switch (state)
        {
            case KeyState.Idle:
                // Flota suavemente en su sitio
                float y = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
                transform.position = new Vector3(startPos.x, y, 0f);
                break;

            case KeyState.Following:
                // Sigue al jugador hacia el offset, visible todo el tiempo
                Vector3 target = playerTr.position + followOffset;
                target.z = 0f;

                // Pequeña oscilación mientras sigue para que se vea vivo
                target.y += Mathf.Sin(Time.time * floatSpeed * 1.5f) * 0.1f;

                transform.position = Vector3.MoveTowards(
                    transform.position, target, magnetSpeed * Time.deltaTime);

                // Una vez cerca del jugador se considera recogida
                float dist = Vector3.Distance(transform.position, playerTr.position);
                if (dist <= collectDist)
                    Collect();
                break;

            case KeyState.Collected:
                // Nada, ya está desactivada
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (state != KeyState.Idle) return;

        CelestePlayer player = other.GetComponent<CelestePlayer>();
        if (player != null)
        {
            playerTr = other.transform;
            state    = KeyState.Following;

            // Desactiva el collider para que no vuelva a triggear
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }
    }

    private void Collect()
    {
        state = KeyState.Collected;
        gameObject.SetActive(false);

        if (targetDoor != null)
            targetDoor.Open();
    }
}