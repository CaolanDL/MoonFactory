using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopOutMenu : MonoBehaviour
{
    public Transform hiddenPosition;
    public Transform shownPosition;
    public float speed;
    bool isHiding = true;
    bool isMoving = false;

    private void Start()
    {
        transform.position = hiddenPosition.position;
    }

    public void Toggle()
    {
        isMoving = true;
        isHiding = !isHiding;
    }

    private void Update()
    {
        if(isMoving)
        {
            if (!isHiding)
            {
                transform.position = Vector3.MoveTowards(transform.position, shownPosition.position, Time.deltaTime * speed );
                if (transform.position.Equals(shownPosition.position)) { isMoving = false; }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, hiddenPosition.position, Time.deltaTime * speed);
                if (transform.position == hiddenPosition.position) { isMoving = false; }
            }
        } 
    } 
}
