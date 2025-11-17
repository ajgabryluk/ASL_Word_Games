using UnityEngine;
using Engine;
using System;

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
        Debug.Log("result: " + result);
        Debug.Log("selectedWord: " + crosswordGenerator.selectedWord);
        if(result == crosswordGenerator.selectedWord)
        {
            crosswordGenerator.ShowWord(result);
            RectTransform panel = transform.parent.GetComponent<RectTransform>();
            panel.anchoredPosition = new Vector2(panel.anchoredPosition.x + 346, 0);
        }
    }
}
