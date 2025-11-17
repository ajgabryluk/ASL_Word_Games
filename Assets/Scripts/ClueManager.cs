using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class ClueManager : MonoBehaviour
{
    public Dictionary<string, string> clues = new Dictionary<string, string>();
    public CrosswordGenerator crosswordGenerator;
    public int numIndex = 0;
    public TMP_Text clueText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "crossword_hints.csv");
        clues = LoadCsvToDict(path);
    }

    private Dictionary<string, string> LoadCsvToDict(string path)
    {
        var dict = new Dictionary<string, string>();

        foreach (string line in File.ReadAllLines(path))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            string[] cols = line.Split(',');

            if (cols.Length < 2) continue;

            string key = cols[0].Trim();
            string value = cols[1].Trim();

            dict[key] = value;
        }

        return dict;
    }

    public void SetClue(string clue)
    {
        clueText.text = clue;
    }

    public void NextClue()
    {
        if(crosswordGenerator.reducedAnswerKey[crosswordGenerator.selectedIndex].Count <= numIndex + 1)
        {
            crosswordGenerator.selectedIndex++;
            if (crosswordGenerator.selectedIndex >= crosswordGenerator.reducedAnswerKey.Count)
            {
                crosswordGenerator.selectedIndex = 0;
            }
            numIndex = 0;
            crosswordGenerator.HighlightDirection(crosswordGenerator.reducedAnswerKey[crosswordGenerator.selectedIndex][numIndex].direction, crosswordGenerator.selectedIndex + 1);
        }
        else
        {
            numIndex++;
            crosswordGenerator.HighlightDirection(crosswordGenerator.reducedAnswerKey[crosswordGenerator.selectedIndex][numIndex].direction, crosswordGenerator.selectedIndex + 1);
        }
    }

    public void PreviousClue()
    {
        if (numIndex == 0)
        {
            crosswordGenerator.selectedIndex--;
            if (crosswordGenerator.selectedIndex < 0)
            {
                crosswordGenerator.selectedIndex = crosswordGenerator.reducedAnswerKey.Count - 1;
            }
            numIndex = crosswordGenerator.reducedAnswerKey[crosswordGenerator.selectedIndex].Count - 1;
            crosswordGenerator.HighlightDirection(crosswordGenerator.reducedAnswerKey[crosswordGenerator.selectedIndex][numIndex].direction, crosswordGenerator.selectedIndex + 1);
        }
        else
        {
            numIndex--;
            crosswordGenerator.HighlightDirection(crosswordGenerator.reducedAnswerKey[crosswordGenerator.selectedIndex][numIndex].direction, crosswordGenerator.selectedIndex + 1);
        }
    }
}
