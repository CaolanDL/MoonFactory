using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPopOut : MonoBehaviour
{
    public Transform hiddenPosition;
    public Transform shownPosition;
    public float speed;
    bool isHiding = true;
    bool isMoving = false;

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
                if (transform.position == shownPosition.position) { isHiding = true; isMoving = false; }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, hiddenPosition.position, Time.deltaTime * speed);
                if (transform.position == hiddenPosition.position) { isHiding = false; isMoving = false; }
            }
        } 
    } 
}
