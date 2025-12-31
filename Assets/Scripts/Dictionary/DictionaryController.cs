using UnityEngine;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Video;

public class DictionaryController : MonoBehaviour
{
    [Header("Settings")]
    public string signListFile = "spellingBeeList";
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
    private List<string> fullList = new List<string>();

    void Start()
    {
        title.text = dictionaryTitle;
        header.color = headerColor;
        sliderFill.color = headerColor;
        rabbit.color = headerColor;
        turtle.color = headerColor;

        LoadWordList();

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
        VideoClip clip = Resources.Load<VideoClip>($"MacarthurBates/{sign}");
        videoPlayer.clip = clip;
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

    public void LoadWordList()
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"WordLists/{signListFile}");

        if (textAsset == null)
        {
            Debug.LogError("Word list not found!");
            return;
        }

        fullList = textAsset.text
            .Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries)
            .ToList();
    }
}
