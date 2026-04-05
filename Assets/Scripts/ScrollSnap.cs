using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollSnap : MonoBehaviour, IEndDragHandler
{
    public ScrollRect scrollRect;
    public RectTransform content;
    private int pageCount;
    private float[] pagePositions;

    public PaginationDots paginationDots;

    void Start()
    {
        pageCount = content.childCount;
        pagePositions = new float[pageCount];
        for (int i = 0; i < pageCount; i++)
        {
            pagePositions[i] = (float)i / (pageCount - 1);
        }

        paginationDots.SetupDots(pageCount);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float currentPos = scrollRect.horizontalNormalizedPosition;
        float closest = pagePositions[0];
        foreach (float pos in pagePositions)
        {
            if (Mathf.Abs(currentPos - pos) < Mathf.Abs(currentPos - closest))
                closest = pos;
        }
        // Smoothly glide to the closest page
        StartCoroutine(LerpToPage(closest));
    }

    System.Collections.IEnumerator LerpToPage(float target)
    {
        float time = 0;
        float start = scrollRect.horizontalNormalizedPosition;

        int targetPageIndex = Mathf.RoundToInt(target * (pageCount - 1));
        paginationDots.UpdateDots(targetPageIndex);

        while (time < 1)
        {
            time += Time.deltaTime * 10f; // Speed of snap
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(start, target, time);
            yield return null;
        }
    }
}