using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMoon : MonoBehaviour
{ 

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime*4, Space.Self);
    }
}
