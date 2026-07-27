using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMangment : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject loadingScreenUI;
    public GameObject OptionsPanel;

    public void PlayGame() {
        loadingScreenUI.SetActive(true);
        SceneManager.LoadSceneAsync(1);
    }

    public void closeOptions()
    { 
        OptionsPanel.SetActive(false);
    }

    public void openOptions()
    {
        OptionsPanel.SetActive(true);
    }
    public void quitGame()
    {
        Application.Quit();
        Debug.Log("Quitting");
    }
}
