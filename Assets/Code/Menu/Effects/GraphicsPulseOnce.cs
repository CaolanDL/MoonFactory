using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

[AddComponentMenu("UI Effects/Pulse Once")]
public class GraphicsPulseOnce : MonoBehaviour
{
    [SerializeField] float scale = 1f;
    [SerializeField] float frequency = 1f;

    Vector3 baseScale = Vector3.one;
    float ElapsedTime = 0f;
    bool isReceeding = false;

    private void Awake()
    {
        baseScale = transform.localScale;
    } 

    public void Set(float scale , float frequency)
    {
        this.scale = scale;
        this.frequency = frequency;
    }

    // Update is called once per frame
    public void Update()
    { 
        var t = (Mathf.Sin(ElapsedTime * frequency) + 1) / 2; // Sine 0 - 1
        var s = 1 + (t * scale);

        var newScale = baseScale * s;

        transform.localScale = newScale; 

        if(t > 0.9) { isReceeding=true; }
        if(isReceeding && t < 0.02f)
        {
            transform.localScale = baseScale;
            Destroy(this); return;
        }

        ElapsedTime += Time.deltaTime;
    }
}
