using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using Engine;
using UnityEngine.Networking;
using TMPro;

public class SpellingBeeManager: MonoBehaviour
{
    [Header("Word Settings")]
    [SerializeField] private string wordList = "spellingBeeList";
    private List<string> consonants = new List<string> { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z" };
    private List<string> vowels = new List<string> { "a", "e", "i", "o", "u" };

    [Header("UI References")]
    public List<GameObject> letterButtons = new List<GameObject>();
    [SerializeField] private TextInput textInput;
    public GameObject answerBoxPrefab;
    public GameObject placeHolderText;

    [Header("New Layout References")]
    [SerializeField] private GameObject horizontalWordsContainer;   // The one with the Pivot X: 1 and Mask
    [SerializeField] private GameObject verticalContentContainer;   // The Content of your Dropdown Scroll View
    [SerializeField] private GameObject dropdownMenuObject;         // The Scroll View itself
    [SerializeField] private TMP_Text dropdownText;                 // Text stating the # of words found

    [Header("Word Data")]
    public List<string> fullList;
    public List<string> answers;
    public List<string> answeredAlready;
    public int answerCount;
    public List<string> letters;
    public int points;
    public int maxPossiblePoints;

    [Header("Difficulty Settings")]
    [Tooltip("Min words in the dictionary that must contain the center letter. Increase to lower difficulty.")]
    public int centerLetterMinFrequency = 1000; 
    [Tooltip("Min number of playable words the puzzle must generate.")]
    public int minPlayableWords = 15;

    [Header("Points")]
    public Image fillImage; // gradient image
    public TMP_Text progressText;
    public GameObject[] stars; // array containing your 3 stars
    public int maxPoints = 40;

    [Header("Levels")]
    public LevelData data;
    

    void Start()
    {   
        // setup Data
        if (LevelDataBridge.isRandomMode)
        {
            LoadWordList();     // Only need the 177k list for random mode
            GenerateLetters();  // Run your 100-attempt loop
        }
        else
        {
            // Use the "Suitcase" we packed in the Home Scene
            InitializePresetLevel(LevelDataBridge.presetLetters, LevelDataBridge.presetWords);
        }

        // 2. Update the UI buttons with whatever letters we ended up with
        for (int i = 0; i < letters.Count; i++)
        {
            if(i < letterButtons.Count)
                letterButtons[i].GetComponent<LetterButtons>().SetLetter(letters[i]);
        }

        // 3. System Initialization
        GameObject.Find("SimpleSLREngine(NoCanvas)").GetComponent<SimpleExecutionEngine>().enabled = true;
        GameObject.Find("Sign Button").GetComponent<CheckSpellngBee>().enabled = true;

        if(dropdownMenuObject != null) dropdownMenuObject.SetActive(false);
        UpdateDropdownText();

        progressText.text = "0/" + maxPoints;
        maxPossiblePoints = calculateMaxPossiblePoints();
        
        string words = string.Join(", ", answers);
        Debug.Log($"Level Loaded! {answers.Count} words found. Max points: {maxPossiblePoints}. Words: {words}");
    }

    public void SubmitWord()
    {
        string submittedWord = textInput.GetCleanWord().ToLower().Trim();

        if (string.IsNullOrEmpty(submittedWord)) return;

        //check if word is in answer list
        if (answers.Contains(submittedWord))
        {
            Debug.Log("Correct! Adding to found list: " + submittedWord);

            MarkWordAsFound(submittedWord);
            textInput.ClearWord();
        } 
        else
        {
            Debug.LogError("Invalid Word: " + submittedWord);
            textInput.ClearWord();
            //UI would also go here to show error
        }
    }

    public void MarkWordAsFound(string word)
    {
        placeHolderText.SetActive(false);
        
        // add to horizotnal display
        GameObject horizBox = Instantiate(answerBoxPrefab, horizontalWordsContainer.transform);

        // set as first sibling so newest word appears first
        horizBox.transform.SetAsFirstSibling(); 
        SetupBox(horizBox, word);

        // add to vertical dropdown list as well
        GameObject vertBox = Instantiate(answerBoxPrefab, verticalContentContainer.transform);
        
        // set as first sibling so newest word appears at top 
        vertBox.transform.SetAsFirstSibling();
        SetupBox(vertBox, word);

        //calculate points
        AddPoints(calculatePoints(word));
        //Debug.Log(points);
        
        //update lists
        answers.Remove(word);
        answeredAlready.Add(word);
        answerCount++;
        UpdateDropdownText();
    }

    public int calculatePoints(string word)
    {
        int calculatedPoints = 0;
        
        // base length points
        if (word.Length == 4)
        {
            calculatedPoints = 1;
        } 
        else if (word.Length > 4)
        {
            calculatedPoints = word.Length;
        }
        else
        {
            Debug.LogError("Word too short for points.");
            return 0;
        }

        //pangram bonus check
        if (isPangram(word))
        {
            calculatedPoints += 7;
        }

        //return points
        return calculatedPoints;
    }

    private int calculateMaxPossiblePoints()
    {
        int totalCount = 0;
        foreach (string answer in answers)
        {
            totalCount += calculatePoints(answer);
        }
        
        return totalCount;
    }

    private bool isPangram(string word)
    {
        // go through each letter in letters list and check is word contains them
        foreach (string letter in letters)
        {
            if (!word.Contains(letter)) 
            {
                return false;
            }
        }
        return true;
    }

    public void UpdateDropdownText()
    {
        if (answerCount == 1)
        {
            dropdownText.text = "You have found " + answerCount + " word";
        } 
        else
        {
            dropdownText.text = "You have found " + answerCount + " words";
        }
    }

    public void ToggleDropdown()
    {
        //if dropdown exists, then either open or close it
        if (dropdownMenuObject != null)
        {
            dropdownMenuObject.SetActive(!dropdownMenuObject.activeSelf);
        }
    }

    private void SetupBox(GameObject go, string word)
    {
        go.transform.name = word;
        AnswerBox script = go.GetComponent<AnswerBox>();
        script.SetAnswer(word);
        script.ShowAnswer();
    }

    private void GenerateLetters()
    {
        int attempts = 0;
        int maxAttempts = 100;
        
        string centerLetter;
        List<string> temp = new List<string>();
        bool pickConsonant = Random.Range(0, 2) == 0;

        while (attempts < maxAttempts) // repeat until a valid letter set is found or failure
        {
            attempts++;

            // 1. Shuffle arrays to ensure randomness
            Shuffle(consonants);
            Shuffle(vowels);

            // Pick a random center letter
            centerLetter = pickConsonant
                ? consonants[Random.Range(0, consonants.Count)]
                : vowels[Random.Range(0, vowels.Count)];

            // Clear previous answers and check center letter
            answers.Clear();
            if (CheckCenterLetter(centerLetter) < centerLetterMinFrequency)
                continue; // invalid center, try again

            // 2. Build letter set
            temp = new List<string>() { centerLetter };

            if (pickConsonant)
            {
                // 5 consonants total → add 4 more
                AddRandomLetters(temp, consonants, 5);
                // Add 2 vowels
                AddRandomLetters(temp, vowels, 7);
            }
            else
            {
                // 3 vowels total → add 2 more
                AddRandomLetters(temp, vowels, 3);
                // Add 4 consonants
                AddRandomLetters(temp, consonants, 7);
            }

            // 3. Check if enough words can be formed
            if (CheckLetterCombinations(temp) >= minPlayableWords)
            {
                letters = temp;
                return; // Success!
            }
        }

        letters = temp;

        Debug.LogError("Could not find a valid puzzle after 100 tries. Lower your minPlayableWords!");
    }

    private void AddRandomLetters(List<string> target, List<string> source, int total)
    {
        List<string> pool = new List<string>(source);
        pool.RemoveAll(l => target.Contains(l)); // avoid duplicates

        while (target.Count < total && pool.Count > 0)
        {
            int index = Random.Range(0, pool.Count);
            target.Add(pool[index]);
            pool.RemoveAt(index);
        }
    }

    private int CheckCenterLetter(string letter)
    {
        char c = letter[0];
        answers = new List<string>(wordsByLetter[c]);
        return answers.Count;
    }

    private int CheckLetterCombinations(List<string> letters)
    {
        // Count available characters
        int[] available = new int[26];
        foreach (var ch in letters)
        {
            char c = ch[0];
            available[c - 'a']++;
        }

        int count = 0;

        for (int i = answers.Count - 1; i >= 0; i--)
        {
            if (!CanBuild(answers[i], available))
                answers.RemoveAt(i);
            else
                count++;
        }

        return count;
    }

    private bool CanBuild(string word, int[] available)
    {
        int[] needed = new int[26];

        foreach (char c in word)
        {
            int i = c - 'a';
            needed[i]++;

            // early-out: if we exceed availability, stop
            if (needed[i] > available[i])
                return false;
        }

        return true;
    }

    private List<string> GetMostCommonConsonants()
    {
        // Count occurrences of each consonant
        Dictionary<string, int> counts = new Dictionary<string, int>();

        // Initialize dictionary with 0 for every consonant
        foreach (string c in consonants)
            counts[c] = 0;

        // Count consonant usage across all words
        foreach (string word in answers)
        {
            foreach (char ch in word)
            {
                string s = ch.ToString();
                if (counts.ContainsKey(s))
                    counts[s]++;
            }
        }

        // Sort by count descending
        return counts
            .OrderByDescending(kvp => kvp.Value)
            .Select(kvp => kvp.Key)
            .ToList();
    }

    private List<string> GetMostCommonVowels()
    {
        // Count occurrences of each vowel
        Dictionary<string, int> counts = new Dictionary<string, int>();

        // Initialize dictionary with 0 for every vowel
        foreach (string v in vowels)
            counts[v] = 0;

        // Count vowel usage across all words
        foreach (string word in answers)
        {
            foreach (char ch in word)
            {
                string s = ch.ToString();
                if (counts.ContainsKey(s))
                    counts[s]++;
            }
        }

        // Sort by count descending
        return counts
            .OrderByDescending(kvp => kvp.Value)
            .Select(kvp => kvp.Key)
            .ToList();
    }
    private void Shuffle(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }

    private Dictionary<char, List<string>> wordsByLetter = new Dictionary<char, List<string>>(); //create dictionaries based on starting letter

    public void LoadWordList()
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"WordLists/{wordList}");

        if (textAsset == null)
        {
            Debug.LogError("Word list not found!");
            return;
        }

        fullList = textAsset.text
            .Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        //pre sorting
        wordsByLetter.Clear();
        for (char c = 'a'; c <= 'z'; c++) wordsByLetter[c] = new List<string>();

        foreach (string word in fullList)
        {
            // add word to the list for every unique letter it contains
            var uniqueChars = word.Distinct();
            foreach (char c in uniqueChars)
            {
                if (wordsByLetter.ContainsKey(c))
                    wordsByLetter[c].Add(word);
            }
        }
    }

    public void ShuffleExistingLetters()
    {
        if (letters == null || letters.Count < 7) return;

        //seperate center letter
        string centerLetter = letters[0];
        List<string> otherLetters = letters.GetRange(1, letters.Count - 1);
        
        //shuffle other letters
        Shuffle(otherLetters);

        //reconstruct letter list 
        letters.Clear();
        letters.Add(centerLetter);
        letters.AddRange(otherLetters);

        //update UI
        for (int i = 0; i < letters.Count; i++)
        {
            if (i < letterButtons.Count)
            {
                letterButtons[i].GetComponent<LetterButtons>().SetLetter(letters[i]);
            }
        }
        
        Debug.Log("Letters Shuffled!");
    }

    public void AddPoints(int amount) {
        points += amount;
        UpdateProgressBar();
    }

    void UpdateProgressBar() {
        
        progressText.text = points + "/" + maxPoints;
        
        float progress = (float)points / (float)maxPoints;
        Debug.Log("progress is " + points + " / " + maxPoints + " = " + progress);

        if (points <= maxPoints)
        {
            float newWidth = progress * 759f;
            fillImage.rectTransform.sizeDelta = new Vector3(newWidth, fillImage.rectTransform.sizeDelta.y);
            
            // Check for Tiers (Stars)
            if (progress >= 0.33f) stars[0].SetActive(true);
            if (progress >= 0.66f) stars[1].SetActive(true);
            if (progress >= 1.00f) stars[2].SetActive(true);
        }
    }


    void InitializePresetLevel(string presetLetters, string[] presetWords)
    {
        // Convert string of letters into the List<string> the game uses
        letters = new List<string>();
        foreach (char c in presetLetters)
        {
            letters.Add(c.ToString().ToLower());
        }

        // Directly assign the answers from the ScriptableObject
        answers = new List<string>(presetWords);
    }


}
