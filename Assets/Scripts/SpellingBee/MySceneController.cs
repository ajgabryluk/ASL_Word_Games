using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneController : MonoBehaviour
{
    public void GoToHome()
    {
        // reset random mode
        LevelDataBridge.isRandomMode = false;

        SceneManager.LoadScene("Home");
    }
}
