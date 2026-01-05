using UnityEngine;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;

public class CheckCrossWord : MonoBehaviour
{
    private SimpleExecutionEngine engine;
    [SerializeField]
    private CrosswordGenerator crosswordGenerator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        engine = GameObject.Find("SimpleSLREngine(NoCanvas)").GetComponent<SimpleExecutionEngine>();
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
            Debug.Log("result: " + result);
            Debug.Log("selectedWord: " + crosswordGenerator.selectedWord);
            engine.recognizer.outputFilters.Clear();
            engine.UpdateFilters(crosswordGenerator.wordList);
            if(result == crosswordGenerator.selectedWord)
            {
                crosswordGenerator.ShowWord(result);
                ResetPosition();
            }
        }  
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
    public void ResetPosition()
    {
        RectTransform panel = transform.parent.GetComponent<RectTransform>();
        panel.anchoredPosition = new Vector2(9999, 0);
    }
}
