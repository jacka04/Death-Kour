using UnityEngine;

public sealed class BalanceoTitulo : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [Range(0f, 50f)] public float amplitud = 10f; // Grados de inclinación
    [Range(0f, 5f)] public float velocidad = 1.5f; // Qué tan rápido se mueve

    private void Update()
    {
        // Calculamos el ángulo usando el tiempo y la función Seno
        float angulo = Mathf.Sin(Time.time * velocidad) * amplitud;

        // Aplicamos la rotación en el eje Z (el que hace el balanceo 2D)
        transform.rotation = Quaternion.Euler(0, 0, angulo);
    }
}