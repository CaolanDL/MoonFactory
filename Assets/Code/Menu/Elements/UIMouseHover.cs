using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public bool IsMouseOver = false;

    public Vector2 pointerPosition = Vector2.zero;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        IsMouseOver = true;
    }

    public virtual void OnMouseHovered()
    {

    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        IsMouseOver = false;
    }

    public void OnPointerMove(PointerEventData pointerEventData)
    {
        pointerPosition = pointerEventData.position;
    }  
}
