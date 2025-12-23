using UnityEngine;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Video;

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
    public TMP_InputField textInput;
    public GameObject videoPage;
    public VideoPlayer videoPlayer;
    public TMP_Text videoTitle;
    public Image sliderFill;
    public Image turtle;
    public Image rabbit;

    void Start()
    {
        title.text = dictionaryTitle;
        header.color = headerColor;
        sliderFill.color = headerColor;
        rabbit.color = headerColor;
        turtle.color = headerColor;
        string path = Path.Combine(Application.streamingAssetsPath, signListFile);
        List<string> fullList = new List<string>(File.ReadAllLines(path));

        foreach (string sign in fullList)
        {
            GameObject button = Instantiate(buttonPrefab, container.transform);
            button.GetComponent<DictionaryButton>().SetSign(sign);
            button.GetComponent<DictionaryButton>().controller = this;
        }
    }

    public void FilterSigns()
    {
        foreach (Transform child in container.transform)
        {
            child.gameObject.SetActive(child.GetComponent<DictionaryButton>().sign.text.ToLower().Contains(textInput.text.ToLower()));
        }
    }

    public void LoadVideoPage(string sign)
    {
        videoPage.SetActive(true);
        videoTitle.gameObject.SetActive(true);
        videoTitle.text = sign;
        textInput.gameObject.SetActive(false);
        videoPlayer.url = Application.streamingAssetsPath + $"/MacarthurBates/{sign}.mp4";
        videoPlayer.Play();
    }

    public void ExitVideoPage()
    {
        videoPlayer.Stop();
        videoPage.SetActive(false);
        videoTitle.gameObject.SetActive(false);
        textInput.gameObject.SetActive(true);
    }

    public void SpeedControl(Slider slider)
    {
        videoPlayer.playbackSpeed = slider.value;
    }
}
