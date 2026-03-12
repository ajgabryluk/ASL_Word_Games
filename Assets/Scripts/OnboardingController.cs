using UnityEngine;
using System.Collections.Generic;

public class OnboardingController : MonoBehaviour
{
    public List<GameObject> pages; // Drag your Page panels here in order
    private int currentIndex = 0;

    void Start()
    {
        // Ensure only the first page is active at the start
        ShowPage(0);
    }

    public void NextPage()
    {
        currentIndex++;

        if (currentIndex < pages.Count)
        {
            ShowPage(currentIndex);
        }
        else
        {
            // Onboarding finished. Close the UI or load the game
            gameObject.SetActive(false); 
            Debug.Log("Onboarding Complete");
        }
    }

    private void ShowPage(int index)
    {
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].SetActive(i == index);
        }
    }
}
