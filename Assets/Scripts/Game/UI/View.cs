using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;

    private RectTransform rectTransform;
    private Rect currentSafeArea;
    private ScreenOrientation currentScreenOrientation = ScreenOrientation.AutoRotation;

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    protected virtual void Start()
    {
        ApplySafeArea();
    }

    protected virtual void Update()
    {
        if (currentScreenOrientation != Screen.orientation || currentSafeArea != Screen.safeArea)
        {
            ApplySafeArea();
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ApplySafeArea()
    {
        if (rectTransform == null)
        {
            return;
        }

        Rect safeArea = Screen.safeArea;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= canvas.pixelRect.width;
        anchorMin.y /= canvas.pixelRect.height;

        anchorMax.x /= canvas.pixelRect.width;
        anchorMax.y /= canvas.pixelRect.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;

        currentSafeArea = Screen.safeArea;
        currentScreenOrientation = Screen.orientation;
    }
}
