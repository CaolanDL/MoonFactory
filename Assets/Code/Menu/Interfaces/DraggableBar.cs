using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DraggableBar : UIMouseHover
{
    public GameObject draggable;

    private RectTransform rectTransform;

    Vector3 offset;

    Vector3 mousePosition;

    Vector3 mouseOffsetPosition;

    bool isDragging = false;

    private void Awake()
    {
        rectTransform = draggable.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (draggable != null)
        {
            mousePosition = Mouse.current.position.value;

            if (IsMouseOver && Mouse.current.leftButton.wasPressedThisFrame)
            {
                offset = draggable.transform.position - mousePosition;
                isDragging = true;
            }

            if (isDragging)
            {
                mouseOffsetPosition = mousePosition + offset;
                draggable.transform.position = mouseOffsetPosition;
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                isDragging = false;
            }



            // Replace this with unity value clamping
            if (rectTransform.rect.position.x < 0)
            {
                rectTransform.rect.position.Set(0, rectTransform.position.y);
            }
            if (rectTransform.rect.position.y < 0)
            {
                rectTransform.rect.position.Set(rectTransform.position.x, 0);
            }

            if (rectTransform.rect.position.x > 1920)
            {
                rectTransform.rect.position.Set(1900, rectTransform.position.y);
            }
            if (rectTransform.rect.position.y > 1080)
            {
                rectTransform.rect.position.Set(rectTransform.position.x, 1060);
            }
        }
    }
}
