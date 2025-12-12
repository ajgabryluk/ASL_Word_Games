using TMPro;
using UnityEngine;

public class AnswerBox : MonoBehaviour
{
    public TMP_Text answer;

    public void SetAnswer(string word)
    {
        answer.text = word;
    }
    public void ShowAnswer()
    {
        answer.color = Color.black;
    }
}
