using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Engine;

public class WordleManager: MonoBehaviour
{
    [SerializeField] 
    private string wordList = "wordleList.txt";
    public List<string> words;
    public string answer = "";
    public List<WordleRow> rows = new List<WordleRow>();
    public int currentRow = 0;

    void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, wordList);
        words = new List<string>(File.ReadAllLines(path));
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
}
