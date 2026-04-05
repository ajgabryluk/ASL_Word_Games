using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectButton : MonoBehaviour
{
    public LevelData data;

    public void OnClick()
    {
        LevelDataBridge.isRandomMode = false;
        LevelDataBridge.presetLetters = data.presetLetters;
        LevelDataBridge.presetWords = data.processedWords.ToArray();
        
        // Change "SpellingBee" to whatever your Game Scene is named
        SceneManager.LoadScene("SignBee_1"); 
    }
}
