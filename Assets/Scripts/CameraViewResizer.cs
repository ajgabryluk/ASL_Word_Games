using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AspectRatioFitter))]
public class CameraViewResizer : MonoBehaviour
{
    private AspectRatioFitter _fitter;
    private RawImage _rawImage;

    void Awake()
    {
        _fitter = GetComponent<AspectRatioFitter>();
        _rawImage = GetComponent<RawImage>();
        
        // Force the mode you requested
        _fitter.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
    }

    void Update()
    {
        // check if the painter or camera has assigned a texture yet
        if (_rawImage.texture != null && _rawImage.texture.width > 0)
        {
            // calculate height/width ratio
            float ratio = (float)_rawImage.texture.height / (float)_rawImage.texture.width;

            // only update if the ratio has changed to save performance
            if (!Mathf.Approximately(_fitter.aspectRatio, ratio))
            {
                _fitter.aspectRatio = ratio;
            }
        }
    }
}