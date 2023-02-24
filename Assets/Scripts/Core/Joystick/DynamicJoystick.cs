using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicJoystick : Joystick
{
    [SerializeField]
    private bool canVisualize = true;

    private void Start()
    {
        HideVirtualControls();
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickImage.rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 position
        ))
        {
            position.x /= joystickImage.rectTransform.sizeDelta.x;
            position.y /= joystickImage.rectTransform.sizeDelta.y;

            Vector2 centerPivot = new Vector2(0.5f, 0.5f);
            position.x += joystickImage.rectTransform.pivot.x - centerPivot.x;
            position.y += joystickImage.rectTransform.pivot.y - centerPivot.y;

            input = Vector2.ClampMagnitude(position, 1.0f);

            float handleAnchoredPositionX = Input.x * joystickImage.rectTransform.sizeDelta.x * 0.5f;
            float handleAnchoredPositionY = Input.y * joystickImage.rectTransform.sizeDelta.y * 0.5f;

            handleImage.rectTransform.anchoredPosition = new Vector2(
                handleAnchoredPositionX,
                handleAnchoredPositionY
            );
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        joystickImage.rectTransform.position = eventData.position;

        OnDrag(eventData);

        if (canVisualize)
        {
            ShowVirtualControls();
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;

        handleImage.rectTransform.anchoredPosition = Vector2.zero;

        if (canVisualize)
        {
            HideVirtualControls();
        }
    }

    private void ShowVirtualControls()
    {
        joystickImage.gameObject.SetActive(true);
    }

    private void HideVirtualControls()
    {
        joystickImage.gameObject.SetActive(false);
    }
}
