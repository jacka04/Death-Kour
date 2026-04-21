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
            if (panelMain) panelMain.SetActive(false);
            if (panelLevelSelect) panelLevelSelect.SetActive(true);
        });
    }

    public void OnBotonOpciones()
    {
        TransitionManager.Instance.TransitionCanvas(() =>
        {
            if (panelMain) panelMain.SetActive(false);
            if (panelOptions) panelOptions.SetActive(true);
        });
    }

    public void OnVolverDesdeLevelSelect()
    {
        TransitionManager.Instance.TransitionCanvas(() =>
        {
            if (panelLevelSelect) panelLevelSelect.SetActive(false);
            if (panelMain) panelMain.SetActive(true);
        });
    }

    public void OnVolverDesdeOpciones()
    {
        TransitionManager.Instance.TransitionCanvas(() =>
        {
            if (panelOptions) panelOptions.SetActive(false);
            if (panelMain) panelMain.SetActive(true);
        });
    }

    public void OnBotonNivel(int index)
    {
        if (index < 0 || index >= levelSceneNames.Length) return;
        TransitionManager.Instance.TransitionScene(levelSceneNames[index]);
    }

    public void OnBotonNivelPorNombre(string sceneName)
    {
        TransitionManager.Instance.TransitionScene(sceneName);
    }

    public void OnBotonVolverAlMenu()
    {
        Time.timeScale = 1f;
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