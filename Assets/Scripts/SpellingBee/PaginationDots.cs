using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PaginationDots : MonoBehaviour
{
    public ScrollRect scrollRect;
    public GameObject dotPrefab;
    public Transform dotContainer;
    public Color activeColor = Color.blue;
    public Color inactiveColor = Color.gray;

    private List<Image> dotImages = new List<Image>();
    private int totalPages;

    public void SetupDots(int pageCount)
    {
        totalPages = pageCount;
        
        // Clear any old dots
        foreach (Transform child in dotContainer) Destroy(child.gameObject);
        dotImages.Clear();

        // Create new dots
        for (int i = 0; i < pageCount; i++)
        {
            GameObject dot = Instantiate(dotPrefab, dotContainer);
            Image img = dot.GetComponent<Image>();
            dotImages.Add(img);
            img.color = inactiveColor;
        }

        UpdateDots(0); // Start at first page
    }

    public void UpdateDots(int currentPage)
    {
        for (int i = 0; i < dotImages.Count; i++)
        {
            dotImages[i].color = (i == currentPage) ? activeColor : inactiveColor;
        }
    }
}