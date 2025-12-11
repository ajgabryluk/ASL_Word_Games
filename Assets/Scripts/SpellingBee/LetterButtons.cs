using TMPro;
using UnityEngine;

public class LetterButtons : MonoBehaviour
{
    public TMP_Text letter;

    public void SetLetter(string letter)
    {
        this.letter.text = letter;
    }

    public void TypeLetter()
    {
        Debug.Log("Typed letter: " + letter.text);
    }
}
