using UnityEngine;

/// <summary>
/// Puerta con candado.
/// Desaparece cuando la llave llama a Open().
/// Asigna este script al GameObject de la puerta (con su Collider).
/// </summary>
public class Door : MonoBehaviour
{
    public void Open()
    {
        // Desactiva el collider para que el jugador pueda pasar
        // y luego desaparece el GameObject
        gameObject.SetActive(false);
    }
}