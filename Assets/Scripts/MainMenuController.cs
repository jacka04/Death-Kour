using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject levelSelectPanel;
    public GameObject settingsPanel;

    public void OpenLevelSelect()
    {
        mainPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    public void OpenSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void BackToMain()
    {
        mainPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void QuitGame() => Application.Quit();
}