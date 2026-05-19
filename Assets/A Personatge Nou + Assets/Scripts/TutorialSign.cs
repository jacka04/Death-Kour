using UnityEngine;
using System.Collections;

public class TutorialSign : MonoBehaviour
{
    [Header("Configuración Visual")]
    [SerializeField] private CanvasGroup canvasGroup; 

    [SerializeField] private float velocidadFade = 2.5f; 

    private Coroutine corrutinaFade;

    void Start()
    {

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
        else
        {
            Debug.LogError("¡Olvidasste arrastrar el Canvas al script en " + gameObject.name + "!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IniciarFade(1f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IniciarFade(0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IniciarFade(1f);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IniciarFade(0f);
        }
    }

    private void IniciarFade(float objetivo)
    {

        if (corrutinaFade != null) StopCoroutine(corrutinaFade);
        corrutinaFade = StartCoroutine(HacerFade(objetivo));
    }

    private IEnumerator HacerFade(float objetivo)
    {
        while (!Mathf.Approximately(canvasGroup.alpha, objetivo))
        {

            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, objetivo, velocidadFade * Time.deltaTime);
            yield return null;
        }
        canvasGroup.alpha = objetivo;
    }
}