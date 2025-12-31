using UnityEngine;
using Engine;
using System.Collections.Generic;
using System.Linq;

public class CheckSpellngBee : MonoBehaviour
{
    [SerializeField]
    private SimpleExecutionEngine engine;
    [SerializeField]
    private SpellingBeeManager spellingBeeManager;
    [SerializeField]
    private TextInput textInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        engine.recognizer.AddCallback("Sign", CheckWord);   
    }

    public void CheckValidWord()
    {
        if(!spellingBeeManager.answers.Contains(textInput.text.text))
        {
            Debug.Log("Not a valid word");
        }
        else
        {
            GameObject.Find("OpenCamera").GetComponent<OpenCamera>().OpenCameraButton();
        }
    }

    public void TriggerRecognizer()
    {
        engine.buffer.TriggerCallbacks();
    }
    public void CheckWord(string result)
    {
        int index = engine.myLoger.LastResult.mapping.IndexOf(textInput.text.text);
        Debug.Log("Predicted Word: " + engine.myLoger.LastResult.mapping[index]);
        Debug.Log("Index: " + index);
        
        if(engine.myLoger.LastResult.probabilities.ToList<float>()[index] > 0.3f)
        {
            Debug.Log("Filtered Result: " + result);
            GameObject.Find(result).GetComponent<AnswerBox>().ShowAnswer();
            ResetPosition();
        }
        else
        {
            Debug.Log("Sign failed please try again");
        }
    }

    public void ResetPosition()
    {
        RectTransform panel = transform.parent.GetComponent<RectTransform>();
        panel.anchoredPosition = new Vector2(panel.anchoredPosition.x + 346, 0);
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

