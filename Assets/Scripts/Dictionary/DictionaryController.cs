using UnityEngine;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System;

public class DictionaryController : MonoBehaviour
{
    [Header("Settings")]
    public string signListFile = "spellingBeeList.txt";
    public string dictionaryTitle = "Dictionary";
    public Color headerColor = Color.black;

    [Header("Dictionary UI Parts")]
    public GameObject container;
    public GameObject buttonPrefab;
    public TMP_Text title;
    public Image header;

    void Start()
    {
        title.text = dictionaryTitle;
        header.color = headerColor;
        string path = Path.Combine(Application.streamingAssetsPath, signListFile);
        List<string> fullList = new List<string>(File.ReadAllLines(path));

        foreach (string sign in fullList)
        {
            GameObject button = Instantiate(buttonPrefab, container.transform);
            button.GetComponent<DictionaryButton>().SetSign(sign);
        }
    }
}
