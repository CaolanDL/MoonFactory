using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine; 

public class DebugUI : MonoBehaviour
{
    public TextMeshProUGUI FPSCounter;
    public TextMeshPro TileCounter;
    public TextMeshPro RoverCounter;

      
    private void FixedUpdate()
    {
        FPSCounter.text = "FPS " + ((int) (1.0f / Time.deltaTime)).ToString();
    }
}
