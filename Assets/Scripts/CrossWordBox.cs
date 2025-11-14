using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor.Search;

public class CrossWordBox : MonoBehaviour
{
    public TMP_Text letterText;
    public TMP_Text numberText;
    public Image backgroundImage;
    public List<CrosswordGenerator.WordLocation> wordLocation;
    public CrosswordGenerator generator;
    public CrosswordGenerator.Direction direction = CrosswordGenerator.Direction.Vertical;

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

    public void HighlightDirection()
    {
        if(numberText.text == "") return;
        //Clear previous highlights
        ClearAllHighlights();

        //Set the direction first
        if(wordLocation.Count == 1)
        {
            direction = wordLocation[0].direction;
        }
        else if(direction == CrosswordGenerator.Direction.Horizontal)
        {
            direction = CrosswordGenerator.Direction.Vertical;
        }
        else
        {
            direction = CrosswordGenerator.Direction.Horizontal;
        }

        //Get parent
        Transform puzzle = transform.parent;
        if(direction == CrosswordGenerator.Direction.Horizontal)
        {   
            CrosswordGenerator.WordLocation word = wordLocation.Find(x => x.direction == CrosswordGenerator.Direction.Horizontal);
            for (int i = word.x; i < word.word.Length + word.x; i++)
            {
                puzzle.Find($"{word.y},{i}").gameObject.GetComponent<CrossWordBox>().backgroundImage.color = Color.cyan;
            }
            ShowWord(word.x, word.y, CrosswordGenerator.Direction.Horizontal);
        }
        else
        {
            CrosswordGenerator.WordLocation word = wordLocation.Find(x => x.direction == CrosswordGenerator.Direction.Vertical);
            for (int i = word.y; i < word.word.Length + word.y; i++)
            {
                puzzle.Find($"{i},{word.x}").gameObject.GetComponent<CrossWordBox>().backgroundImage.color = Color.cyan;
            }
            ShowWord(word.x, word.y, CrosswordGenerator.Direction.Vertical);
        }
    }

    public void ShowWord(int x, int y, CrosswordGenerator.Direction dir)
    {
        if (dir == CrosswordGenerator.Direction.Horizontal)
        {
            for (int i = x; i < x + wordLocation[0].word.Length; i++)
            {
                Debug.Log($"{y},{i}");
                transform.parent.Find($"{y},{i}").gameObject.GetComponent<CrossWordBox>().letterText.gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = y; i < y + wordLocation[0].word.Length; i++)
            {
                transform.parent.Find($"{i},{x}").gameObject.GetComponent<CrossWordBox>().letterText.gameObject.SetActive(true);
            }
        }
    }
    public void ClearAllHighlights()
    {
        Transform puzzle = transform.parent;
        for (int i = 0; i < puzzle.childCount; i++)
        {
            if(puzzle.GetChild(i).gameObject.GetComponent<CrossWordBox>() == null) continue;
            puzzle.GetChild(i).gameObject.GetComponent<CrossWordBox>().backgroundImage.color = Color.white;
        }
    }
}
