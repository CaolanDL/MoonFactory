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



            // Clamp interface position to screen bounds
            if (rectTransform.anchoredPosition.x < 0)
            {
                rectTransform.anchoredPosition = new Vector3(0, rectTransform.anchoredPosition.y, 0);
            }
            if (rectTransform.anchoredPosition.y < 0)
            {
                rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, 20, 0);
            } 
            if (rectTransform.anchoredPosition.x > 1920)
            {
                rectTransform.anchoredPosition = new Vector3(1860, rectTransform.anchoredPosition.y, 0);
            }
            if (rectTransform.anchoredPosition.y > 1080)
            {
                rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, 1080, 0);
            }
        }
    }
}
