using TMPro;
using UnityEngine;

public class DictionaryButton : MonoBehaviour
{
    public TMP_Text sign;
    
    public void SetSign(string s)
    {
        sign.text = s;
    }
    public void Click()
    {
        Debug.Log("Clicked: " + sign.text);
    }
}
