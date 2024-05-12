using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayAfterDelay : MonoBehaviour
{ 
    [Tooltip("In Seconds"), SerializeField] float delay;
    float timer;
    Vector3 baseScale = Vector3.one; 

    private void Awake()
    {
        baseScale = transform.localScale;
        transform.localScale = Vector3.zero;
        timer = delay;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            transform.localScale = baseScale;
            Destroy(this);
            return;
        }
    }
}
