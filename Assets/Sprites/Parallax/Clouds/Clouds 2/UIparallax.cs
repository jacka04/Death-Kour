using UnityEngine;
using UnityEngine.UI;

public class UIParallax : MonoBehaviour
{
    [Header("Asigna tus RawImages aquí")]
    public RawImage fondo;
    public RawImage miniNubes;
    public RawImage mediaNubes;
    public RawImage nubesGrandes;

    [Header("Velocidades (Prueba valores bajos)")]
    public float velFondo = 0.005f;
    public float velMini = 0.01f;
    public float velMedia = 0.02f;
    public float velGrandes = 0.04f;

    void Update()
    {
        // Movemos el UV Rect de cada imagen de forma independiente
        Mover(fondo, velFondo);
        Mover(miniNubes, velMini);
        Mover(mediaNubes, velMedia);
        Mover(nubesGrandes, velGrandes);
    }

    void Mover(RawImage img, float v)
    {
        if (img != null)
        {
            Rect r = img.uvRect;
            // Sumamos al eje X para que se mueva hacia la derecha
            r.x += v * Time.deltaTime;
            img.uvRect = r;
        }
    }
}