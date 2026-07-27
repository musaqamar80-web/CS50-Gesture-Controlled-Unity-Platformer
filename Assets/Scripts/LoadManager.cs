using UnityEngine.SceneManagement;

public static class LoadManager
{
    public static string sceneToLoad;

    public static void GoToLevel(string targetScene)
    {
        sceneToLoad = targetScene;
        SceneManager.LoadScene("Loading");
    }
}
