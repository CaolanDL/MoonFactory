using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    Image image;
    [SerializeField] float time;
    [SerializeField] float initalDelay = 0f;

    private void Awake()
    {
        image = GetComponent<Image>();
        if(initalDelay <= 0f) image.CrossFadeAlpha(0, time, false);
    }

    bool fading = false;
    private void Update()
    { 
        if(initalDelay > 0f) initalDelay -= Time.deltaTime;
        if (initalDelay <= 0f && !fading)
        {
            fading = true;
            image.CrossFadeAlpha(0, time, false);
        }
        if (image.color.a < 0.005f) { Destroy(gameObject); }
    }
}
