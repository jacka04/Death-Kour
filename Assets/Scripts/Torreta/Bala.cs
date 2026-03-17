using UnityEngine;
using PlayerSystem;

public class Bala : MonoBehaviour
{
    public float velocidad = 8f;
    public float tiempoDeVida = 5f;

    void Start()
    {
        Destroy(gameObject, tiempoDeVida);
    }

    void Update()
    {
        transform.Translate(Vector2.left * velocidad * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            PlayerController3 jugador = otro.GetComponent<PlayerController3>();

            if (jugador != null)
            {
                jugador.Die();
            }

            Destroy(gameObject);
        }
        else if (otro.CompareTag("Suelo"))
        {
            Destroy(gameObject);
        }
    }
}