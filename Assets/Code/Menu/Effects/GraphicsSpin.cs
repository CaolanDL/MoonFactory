using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsSpin : MonoBehaviour
{
    [SerializeField] float speed;

    // Update is called once per frame
    public void Update()
    {
        transform.Rotate(0, 0, Time.deltaTime * speed) ;
    }
}
