using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CrossWordBox : MonoBehaviour
{
    public TMP_Text letterText;
    public TMP_Text numberText;
    public Image backgroundImage;
    public CrosswordGenerator generator;
    public CrosswordGenerator.Direction direction;

    void Start()
    {
        generator = GameObject.Find("Crossword").GetComponent<CrosswordGenerator>();
        GetComponent<Button>().onClick.AddListener(delegate {generator.HighlightDirection(direction, numberText.text == "" ? -1 : int.Parse(numberText.text)); });
    }
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
    
    public void HighlightBox()
    {
        backgroundImage.color = Color.cyan;
    }
    public void ChangeDirection(CrosswordGenerator.Direction dir)
    {
        direction = dir;
    }   
}
