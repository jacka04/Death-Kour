using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Canvas / Panels")]
    [SerializeField] private GameObject panelMain;
    [SerializeField] private GameObject panelOptions;
    [SerializeField] private GameObject panelLevelSelect;

    [Header("Escenas")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string[] levelSceneNames;

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

    public void OnBotonVolverAlMenu()
    {
        TransitionManager.Instance.TransitionScene(mainMenuSceneName);
    }

    public void OnBotonSalir()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}