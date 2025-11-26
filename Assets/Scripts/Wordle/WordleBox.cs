using UnityEngine;
using TMPro;
using UnityEngine.UI;
using JetBrains.Annotations;

public class WordleBox : MonoBehaviour
{
    public TMP_Text letterText;
    public Image backgroundImage;

    public void MarkCorrect(char letter) {
        backgroundImage.color = Color.green;
        letterText.text = letter.ToString();
    }

    public void MarkIncorrect(char letter){
        backgroundImage.color = Color.red;
        letterText.text = letter.ToString();
    }

    public void MarkIncorrectPosition(char letter){
        backgroundImage.color = Color.yellow;
        letterText.text = letter.ToString();
    }
}