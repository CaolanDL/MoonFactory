using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeOnAwake : MonoBehaviour
{
    [SerializeField] float scale = 0.02f;
    [SerializeField] float frequency = 20f;

    Vector3 startPosition;
    float ElapsedTime = 0f;
    bool isReceeding = false;

    private void Awake()
    {
        startPosition = transform.position;
    }

    public void Set(float scale, float frequency)
    {
        this.scale = scale;
        this.frequency = frequency;
    }

    // Update is called once per frame
    public void Update()
    {
        var t = (Mathf.Sin(ElapsedTime * frequency) + 1) / 2; // Sine 0 - 1
        var s = (t * scale);

        var newPosition = startPosition + (Vector3.up * s);

        transform.position = newPosition;

        if (t > 0.9) { isReceeding = true; }
        if (isReceeding && t < 0.02f)
        {
            transform.position = startPosition;
            Destroy(this); return;
        }

        ElapsedTime += Time.deltaTime;
    }
}
