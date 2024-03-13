using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine; 

public class DebugUI : MonoBehaviour
{
    public TextMeshProUGUI FPSCounter;
    public TextMeshPro ItemCounter;
    public TextMeshPro RoverCounter;

    private int FPS;


    private void Update()
    {
        FPS = (int)(1.0f / Time.smoothDeltaTime);
    }

    private void FixedUpdate()
    {
        FPSCounter.text = $"FPS {FPS}";


    }
}
