using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GenericTooltipHandler : MonoBehaviour
{
    [SerializeField] TMP_Text Header;
    [SerializeField] TMP_Text Content; 

    private void Awake()
    {
        transform.position = Input.mousePosition; 
    }

    Vector3 velocity = Vector3.zero;
    private void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, Input.mousePosition, ref velocity, 0.1f);
    }

    public void SetAlignment(Vector2 alignment)
    {
        RectTransform rectTransform = GetComponent<RectTransform>(); 
        rectTransform.pivot = (alignment);
    }
    public void SetContent(string content)
    {
        Content.text = content;
    }
    public void SetHeader(string header)
    {
        Header.text = header;
    }
}
