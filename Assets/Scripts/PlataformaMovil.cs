using UnityEngine;

public class PlataformaMovil : MonoBehaviour
{
    public Transform puntoA;
    public Transform puntoB;
    public float velocidad = 3f;
    private Vector3 destinoActual;

    void Start()
    {
        if (puntoA != null && puntoB != null)
        {
            transform.position = puntoA.position;
            destinoActual = puntoB.position;
        }
    }

    void Update()
    {
        if (puntoA == null || puntoB == null) return;

        transform.position = Vector3.MoveTowards(transform.position, destinoActual, velocidad * Time.deltaTime);

        if (Vector3.Distance(transform.position, destinoActual) < 0.05f)
        {
            destinoActual = (destinoActual == puntoA.position) ? puntoB.position : puntoA.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}