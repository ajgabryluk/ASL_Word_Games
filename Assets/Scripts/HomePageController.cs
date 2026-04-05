using UnityEngine;
using UnityEngine.SceneManagement;

public class HomePageController : MonoBehaviour
{
    public void StartRandomMode()
    {
        // 1. Tell the bridge we want the random generator
        LevelDataBridge.isRandomMode = true;
        
        // 2. Clear out any leftover preset data just in case
        LevelDataBridge.presetLetters = "";
        LevelDataBridge.presetWords = null;

        // 3. Load the game scene
        // Make sure "SpellingBee" matches your scene name exactly!
        SceneManager.LoadScene("SignBee_1");
    }
}
