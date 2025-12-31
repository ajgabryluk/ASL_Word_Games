using UnityEngine;
using UnityEngine.UI;

public class NavigationResizer : MonoBehaviour
{
    public GridLayoutGroup gridLayout;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RectTransform parentRect = GetComponent<RectTransform>();
        int columns = 4; // or your desired number
        float spacing = gridLayout.spacing.x;
        float padding = gridLayout.padding.left + gridLayout.padding.right;
        float cellWidth = (parentRect.rect.width - padding - spacing * (columns - 1)) / columns;
        gridLayout.cellSize = new Vector2(cellWidth, gridLayout.cellSize.y); 
    }
}
