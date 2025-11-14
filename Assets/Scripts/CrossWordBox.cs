using UnityEngine;
using TMPro;

public class CrossWordBox : MonoBehaviour
{
    public TMP_Text letterText;
    public TMP_Text numberText;

    public void SetNumber(string number)
    {
        numberText.text = number;
    }

    public void SetLetter(char letter)
    {
        letterText.text = letter.ToString();
    }
    public void ShowLetter()
    {
        letterText.gameObject.SetActive(true);        
    }

}
