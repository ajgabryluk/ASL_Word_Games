using TMPro;
using UnityEngine;

public class TextInput : MonoBehaviour
{
    public TMP_Text text;

    public void AddLetter(string letter) {
        text.text += letter;
    }

    public void DeleteLetter() {
        text.text = text.text.Substring(0, text.text.Length - 1);
    }

    public void ClearWord() {
        text.text = "";
    }
}
