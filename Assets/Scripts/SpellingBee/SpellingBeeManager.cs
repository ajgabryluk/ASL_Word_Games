using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using Engine;
using UnityEngine.Networking;

public class SpellingBeeManager: MonoBehaviour
{
    [SerializeField] 
    private string wordList = "spellingBeeList";
    private List<string> consonants = new List<string> { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z" };
    private List<string> vowels = new List<string> { "a", "e", "i", "o", "u" };
    public List<GameObject> letterButtons = new List<GameObject>();
    public List<string> fullList;
    public List<string> answers;
    public List<string> answeredAlready;
    public List<string> letters;
    public GameObject answerBoxPrefab;
    public GameObject answerDisplay;

    [SerializeField] 
    private TextInput textInput;

    void Start()
    {
        LoadWordList();
        GenerateLetters();

        for(int i = 0; i < letters.Count; i++)
        {
            letterButtons[i].GetComponent<LetterButtons>().SetLetter(letters[i]);
        }

        GameObject.Find("SimpleSLREngine(NoCanvas)").GetComponent<SimpleExecutionEngine>().enabled = true;
        GameObject.Find("Sign Button").GetComponent<CheckSpellngBee>().enabled = true;
    }

    public void SubmitWord()
    {
        string submittedWord = textInput.text.text.ToLower().Trim();

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
        // create answer box when user finds word
        GameObject answerBox = Instantiate(answerBoxPrefab, answerDisplay.transform);
        answerBox.transform.name = word;
        
        AnswerBox script = answerBox.GetComponent<AnswerBox>();
        script.SetAnswer(word);
        //make answer visible
        script.ShowAnswer(); 
        
        //add found word to list 
        answers.Remove(word);
        answeredAlready.Add(word);
    }

    private void GenerateLetters()
    {
        string centerLetter;
        List<string> temp;
        bool pickConsonant = Random.Range(0, 2) == 0;

        while (true) // repeat until a valid letter set is found
        {
            // 1. Shuffle arrays to ensure randomness
            Shuffle(consonants);
            Shuffle(vowels);

            // Pick a random center letter
            centerLetter = pickConsonant
                ? consonants[Random.Range(0, consonants.Count)]
                : vowels[Random.Range(0, vowels.Count)];

            // Clear previous answers and check center letter
            answers.Clear();
            if (CheckCenterLetter(centerLetter) < 67)
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
            if (CheckLetterCombinations(temp) >= 6)
                break; // success
            else
                answers.Clear(); // retry
        }

        letters = temp;
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
        int count = 0;
        foreach(string word in fullList)
        {
            if(word.Contains(letter))
            {
                count++;
                answers.Add(word);
            }
        }
        return count;
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
    }
}
