using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public List<GameObject> menus = new List<GameObject>();
    public void LoadMenu(GameObject menu)
    {
        foreach (GameObject m in menus)
        {
            m.SetActive(false);
        }
        menu.SetActive(true);
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
