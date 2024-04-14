using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    Slider slider;

    float progress = 0;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void SetProgress(float progress)
    {
        this.progress = progress; 
        slider.value = progress;
    }
} 