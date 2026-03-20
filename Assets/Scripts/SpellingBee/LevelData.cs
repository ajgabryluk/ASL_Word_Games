using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "NewLevel", menuName = "SpellingBee/LevelData")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public string presetLetters; // 7 letters, first one is the center



    //words
    [TextArea(3, 10)]
    public string rawWordList; //list seperated by commas

    // This is what the game actually uses
    [HideInInspector] // We hide this so you don't have to look at it in the Inspector
    public List<string> processedWords = new List<string>();

    // This runs automatically whenever you change something in the Inspector
    private void OnValidate()
    {
        if (!string.IsNullOrEmpty(rawWordList))
        {
            // Split by comma OR space, remove empty entries, and trim whitespace
            processedWords = rawWordList
                .Split(new[] { ',', ' ', '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.Trim().ToLower())
                .Distinct() 
                .ToList();
        }
    }
}
