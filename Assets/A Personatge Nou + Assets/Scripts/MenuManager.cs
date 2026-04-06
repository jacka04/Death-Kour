using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Canvas / Panels")]
    [SerializeField] private GameObject panelMain;
    [SerializeField] private GameObject panelOptions;
    [SerializeField] private GameObject panelLevelSelect;

    [Header("Nombres de escenas")]
    [SerializeField] private string[] levelSceneNames;

    // ── Navegación entre panels ─────────────────────────────────────────────

    public void OnBotonJugar()
    {
        TransitionManager.Instance.TransitionCanvas(() =>
        {
            panelMain.SetActive(false);
            panelLevelSelect.SetActive(true);
        });
    }

    public void OnBotonOpciones()
    {
        TransitionManager.Instance.TransitionCanvas(() =>
        {
            panelMain.SetActive(false);
            panelOptions.SetActive(true);
        });
    }

    public void OnVolverDesdeLevelSelect()
    {
        TransitionManager.Instance.TransitionCanvas(() =>
        {
            panelLevelSelect.SetActive(false);
            panelMain.SetActive(true);
        });
    }

    public void OnVolverDesdeOpciones()
    {
        TransitionManager.Instance.TransitionCanvas(() =>
        {
            panelOptions.SetActive(false);
            panelMain.SetActive(true);
        });
    }

    // ── Carga de niveles ────────────────────────────────────────────────────

    public void OnBotonNivel(int index)
    {
        if (index < 0 || index >= levelSceneNames.Length)
        {
            Debug.LogWarning($"Índice de nivel {index} fuera de rango.");
            return;
        }

        TransitionManager.Instance.TransitionScene(levelSceneNames[index]);
    }

    public void OnBotonNivelPorNombre(string sceneName)
    {
        TransitionManager.Instance.TransitionScene(sceneName);
    }

    // ── Salir de la aplicación ──────────────────────────────────────────────

    public void OnBotonSalir()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}