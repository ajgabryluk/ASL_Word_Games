using TMPro;
using UnityEngine;

public class DictionaryButton : MonoBehaviour
{
    public TMP_Text sign;
    public DictionaryController controller;
    
    public void SetSign(string s)
    {
        sign.text = s;
    }
    public void Click()
    {
        controller.LoadVideoPage(sign.text);
    }
}
