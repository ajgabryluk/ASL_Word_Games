using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;
using Engine;

public class WordleManager: MonoBehaviour
{
    [SerializeField] 
    private string wordList = "wordleList";
    public List<string> words;
    public string answer = "";
    public List<WordleRow> rows = new List<WordleRow>();
    public int currentRow = 0;

    void Start()
    {
        LoadWordList();
        Shuffle(words);
        answer = words[0];

        GameObject.Find("SimpleSLREngine(NoCanvas)").GetComponent<SimpleExecutionEngine>().enabled = true;
        GameObject.Find("HoldToSign").GetComponent<CheckWordle>().enabled = true;
    }

    private void Shuffle(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }
    
    public void LoadWordList()
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"WordLists/{wordList}");

        if (textAsset == null)
        {
            Debug.LogError("Word list not found!");
            return;
        }

        words = textAsset.text
            .Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries)
            .ToList();
    }
}
