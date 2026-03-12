using TMPro;
using UnityEngine;

public class LetterButtons : MonoBehaviour
{
    public TMP_Text letter;
    public TextInput textInput;

    public void SetLetter(string letter)
    {
        this.letter.text = letter;
    }

    public void TypeLetter()
    {
        textInput.AddLetter(letter.text);
        Debug.Log(letter.text);
    }
}
