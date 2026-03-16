using TMPro;
using UnityEngine;
using System.Collections;

public class TextInput : MonoBehaviour
{
    public TMP_Text text;
    private string currentInput = "";   // actual word not including the text indicator
    private bool isFlashing = false;    // is the text indicator flashing
    private string cursorChar = "|";    // the text indicator

    void Start()
    {
        // start flashing text indicator
        StartCoroutine(FlashCursor());
    }

    public void AddLetter(string letter) {
        currentInput += letter;
        UpdateDisplayText();
        
    }

    // 
    public void DeleteLetter() {
        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            UpdateDisplayText();
        }
    }

    public void ClearWord() {
        currentInput = "";
        UpdateDisplayText();
    }

    public string GetCleanWord()
    {
        return currentInput;
    }

    private void UpdateDisplayText()
    {
        if (!string.IsNullOrEmpty(currentInput)) 
        {
            text.text = currentInput;
        } 
        else
        {
            text.text = (isFlashing ? cursorChar : "");
        }
        //Debug.Log(currentInput);
    }

    IEnumerator FlashCursor()
    {
        while (true)
        {
            isFlashing = !isFlashing;
            UpdateDisplayText();
            yield return new WaitForSeconds(0.5f); // adjust speed here
        }
    }
}
