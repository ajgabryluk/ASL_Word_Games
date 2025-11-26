using UnityEngine;
using Engine;
using System.Collections.Generic;
using System.Linq;
using Common;

public class CheckWordle : MonoBehaviour
{
    [SerializeField]
    private SimpleExecutionEngine engine;
    [SerializeField]
    private WordleManager wordleManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        engine.recognizer.AddCallback("Sign", CheckWord);   
    }

    public void TriggerRecognizer()
    {
        engine.buffer.TriggerCallbacks();
    }
    public void CheckWord(string result)
    {
        List<string> topHalfMappings = GetTopHalfMappings(engine.myLoger.LastResult.mapping, engine.myLoger.LastResult.probabilities.ToList<float>());
        Debug.Log("Top Half Mappings: " + string.Join(", ", topHalfMappings));
        
        if(topHalfMappings.Count >= 6)
        {
            Debug.Log("Unfiltered Result: " + result);
            engine.recognizer.outputFilters.Clear();
            engine.UpdateFilters(topHalfMappings);
            TriggerRecognizer();
        }
        else
        {
            engine.recognizer.outputFilters.Clear();
            engine.UpdateFilters(wordleManager.words);
            Debug.Log("Filtered Result: " + result);
        }
        // for(int i = 0; i < result.Length; i++)
        // {
        //     if(result[i] == wordleManager.answer[i])
        //     {
        //         wordleManager.rows[wordleManager.currentRow].boxes[i].MarkCorrect(result[i]);
        //     }
        //     else if(wordleManager.answer.Contains(result[i]))
        //     {
        //         wordleManager.rows[wordleManager.currentRow].boxes[i].MarkIncorrectPosition(result[i]);
        //     }
        //     else
        //     {
        //         wordleManager.rows[wordleManager.currentRow].boxes[i].MarkIncorrect(result[i]);
        //     }
        // }
        // wordleManager.currentRow++;
        // RectTransform panel = transform.parent.GetComponent<RectTransform>();
        // panel.anchoredPosition = new Vector2(panel.anchoredPosition.x + 346, 0);
    }

    public static List<string> GetTopHalfMappings(List<string> mappings, List<float> probabilities)
    {
        // Pair mapping + prob
        var paired = new List<(string mapping, float prob)>(mappings.Count);

        for (int i = 0; i < mappings.Count; i++)
            paired.Add((mappings[i], probabilities[i]));

        // Sort descending by probability
        paired.Sort((a, b) => b.prob.CompareTo(a.prob));

        // Take top 50%
        int countToTake = paired.Count / 2;

        var result = new List<string>(countToTake);

        for (int i = 0; i < countToTake; i++)
            result.Add(paired[i].mapping);

        return result;
    }
}

