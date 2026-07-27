using UnityEngine;
using UnityEngine.SceneManagement;

public class lvl1SManager : MonoBehaviour
{
    public void back()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
