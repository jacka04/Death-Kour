using UnityEngine;
using System.Collections;

public class MovingElevator : MonoBehaviour
{
    public float distanciaASubir = 5f;
    public float velocidad = 2f;
    public float tiempoEsperaArriba = 2f;

    private bool moviéndose = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !moviéndose)
        {
            StartCoroutine(CicloPlataforma());
        }
    }

    IEnumerator CicloPlataforma()
    {
        moviéndose = true;

        Vector2 posicionInicial = transform.position;
        Vector2 posicionObjetivo = posicionInicial + Vector2.up * distanciaASubir;

        // SUBIDA
        float progreso = 0;
        while (progreso < 1)
        {
            progreso += Time.deltaTime * (velocidad / distanciaASubir);
            transform.position = Vector2.Lerp(posicionInicial, posicionObjetivo, progreso);
            yield return null;
        }
        transform.position = posicionObjetivo;

        // ESPERA
        yield return new WaitForSeconds(tiempoEsperaArriba);

        // BAJADA
        progreso = 0;
        while (progreso < 1)
        {
            progreso += Time.deltaTime * (velocidad / distanciaASubir);
            transform.position = Vector2.Lerp(posicionObjetivo, posicionInicial, progreso);
            yield return null;
        }
        transform.position = posicionInicial;

        moviéndose = false;
    }
}