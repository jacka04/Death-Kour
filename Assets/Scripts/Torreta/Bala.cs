using UnityEngine;
using PlayerSystem; // Importante para poder usar el PlayerController

public class Bala : MonoBehaviour
{
    public float velocidad = 8f;
    public float tiempoDeVida = 5f;

    void Start()
    {
        // Para que la bala no se quede volando infinitamente si no choca con nada
        Destroy(gameObject, tiempoDeVida);
    }

    void Update()
    {
        // Mueve la bala hacia adelante (derecha local del objeto)
        transform.Translate(Vector2.right * velocidad * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            PlayerController jugador = otro.GetComponent<PlayerController>();

            if (jugador != null)
            {
                jugador.Die();
            }

            Destroy(gameObject);
        }
        
        if (otro.CompareTag("Suelo"))
        {
            Destroy(gameObject);
        }
    }
}