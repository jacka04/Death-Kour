using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject levelSelectPanel;

    public void OpenLevelSelect()
    {
        mainPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    public void BackToMain()
    {
        mainPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
    }
}