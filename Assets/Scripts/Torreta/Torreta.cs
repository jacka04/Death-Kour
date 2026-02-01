using UnityEngine;

public class Torreta : MonoBehaviour
{
    public GameObject balaPrefab;
    public Transform puntoDeDisparo;
    public float tiempoParaDisparar = 2.0f;

    private float cronometro;

    void Update()
    {
        cronometro += Time.deltaTime;

        if (cronometro >= tiempoParaDisparar)
        {
            Instantiate(balaPrefab, puntoDeDisparo.position, puntoDeDisparo.rotation);
            
            cronometro = 0;
        }
    }
}