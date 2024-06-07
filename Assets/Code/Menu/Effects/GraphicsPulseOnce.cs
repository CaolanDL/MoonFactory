using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

[AddComponentMenu("UI Effects/Pulse Once")]
public class GraphicsPulseOnce : MonoBehaviour
{
    [SerializeField] float scale = 0.1f;
    [SerializeField] float frequency = 16f;

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
        var t = ElapsedTime * frequency - (90 * Mathf.Deg2Rad);
        var s01 = (Mathf.Sin(t) + 1) / 2; // Sine 0 - 1
        var s = 1 + (s01 * scale);

        var newScale = baseScale * s;

        transform.localScale = newScale;
         
        if(t > 270 * Mathf.Deg2Rad)
        {
            transform.localScale = baseScale;
            Destroy(this); return;
        }

        ElapsedTime += Time.deltaTime;
    }

    private void OnDisable()
    {
        transform.localScale=baseScale;
    }
    private void OnDestroy()
    {
        transform.localScale = baseScale;
    }
}
