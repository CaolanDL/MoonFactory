using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("UI Effects/Pulse")]
public class GraphicsPulse : MonoBehaviour
{
    [SerializeField] float scale; 
    [SerializeField] float frequency;

    Vector3 baseScale = Vector3.one;

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    // Update is called once per frame
    public void Update()
    {
        var t = (Mathf.Sin(Time.time * frequency)+1)/2; // Sine 0 - 1
        var s = 1 + (t * scale); 

        var newScale = baseScale * s;

        transform.localScale = newScale;
    }

    private void OnDisable()
    {
        transform.localScale = baseScale;
    }
}
