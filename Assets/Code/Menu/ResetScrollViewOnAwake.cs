using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ResetScrollViewOnAwake : MonoBehaviour
{
    public ScrollRect scrollView;

    private void Awake()
    {
        scrollView = GetComponent<ScrollRect>();
    }

    // Start is called before the first frame update
    void Start()
    {
        scrollView.normalizedPosition = new Vector2(0,1);
    }
}
