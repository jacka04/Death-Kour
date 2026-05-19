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
    [Tooltip("Duración máxima del sonido de apertura (se cortará al pasar este tiempo)")]
    [SerializeField] private float audioDuration = 2.0f;

    [Tooltip("Tiempo de inicio del audio (para saltarse silencios al principio)")]
    [SerializeField] private float audioStartOffset = 0f;


    private bool isOpening = false;



    public void Open()
    {
        if (isOpening) return;
        isOpening = true;

        if (openSound != null)
        {
            GameObject tempAudio = new GameObject("TempAudio_Door");
            tempAudio.transform.position = transform.position;
            AudioSource source = tempAudio.AddComponent<AudioSource>();
            source.clip = openSound;
            source.volume = volume;
            
            // Recortar el inicio del audio si se especifica un offset
            if (audioStartOffset > 0 && audioStartOffset < openSound.length)
                source.time = audioStartOffset;

            source.Play();


            // Esto es lo que "acorta" el audio si el archivo es muy largo
            Destroy(tempAudio, audioDuration);
        }

        // Se abre INSTANTÁNEAMENTE
        gameObject.SetActive(false);
    }


}