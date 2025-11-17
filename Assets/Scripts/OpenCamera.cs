using UnityEngine;

public class OpenCamera : MonoBehaviour
{
    public RectTransform panel;
    public void OpenCameraButton()
    {
        panel.anchoredPosition = new Vector2(panel.anchoredPosition.x - 346, 0);
    }
}
