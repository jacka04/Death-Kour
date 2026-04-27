using UnityEngine;

public sealed class BalanceoTitulo : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [Range(0f, 50f)] public float amplitud = 10f; 
    [Range(0f, 5f)] public float velocidad = 1.5f; 

    private void Update()
    {
        
        float angulo = Mathf.Sin(Time.time * velocidad) * amplitud;

        
        transform.rotation = Quaternion.Euler(0, 0, angulo);
    }
}