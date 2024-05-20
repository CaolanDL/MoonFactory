using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    Image image;
    [SerializeField] float time;
     

    private void Awake()
    {
        image = GetComponent<Image>();
        image.CrossFadeAlpha(0, time, false);
    }
}
