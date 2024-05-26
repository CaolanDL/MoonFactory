using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

// Fixes the broken mouse scroll functionality for scroll view

public class ScrollWheelFix : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IScrollHandler
{
    ScrollRect scrollView;
    bool isMouseOver = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }

    public void OnScroll(PointerEventData eventData)
    {
        if(isMouseOver && scrollView != null)
        {
            scrollView.verticalNormalizedPosition += eventData.scrollDelta.y;
            Debug.Log(eventData.scrollDelta.y);
        }
    }

    private void OnDisable()
    {
        isMouseOver = false;
    }

    private void Awake()
    {
        scrollView = GetComponent<ScrollRect>();    
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
