using UnityEngine;

/// <summary>
/// Puerta con candado.
/// Desaparece cuando la llave llama a Open().
/// Asigna este script al GameObject de la puerta (con su Collider).
/// </summary>
public class Door : MonoBehaviour
{
    [SerializeField] private AudioClip openSound;
    [SerializeField] [Range(0f, 1f)] private float volume = 1f;
    [Tooltip("Tiempo en segundos para detener el sonido (útil si el audio es muy largo)")]
    [SerializeField] private float stopAfterSeconds = 2.5f;

    public void Open()
    {
        if (openSound != null)
        {
            // Creamos un objeto temporal para el sonido para poder controlar su duración
            GameObject tempAudio = new GameObject("TempAudio");
            tempAudio.transform.position = transform.position;
            AudioSource source = tempAudio.AddComponent<AudioSource>();
            source.clip = openSound;
            source.volume = volume;
            source.Play();

            // Destruye el objeto (y detiene el sonido) después del tiempo especificado
            Destroy(tempAudio, stopAfterSeconds);
        }

        // Desactiva el collider para que el jugador pueda pasar
        // y luego desaparece el GameObject
        gameObject.SetActive(false);
    }
}