using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopOutMenu : MonoBehaviour
{
    public Transform hiddenPosition;
    public Transform shownPosition;
    public float speed = 3000;
    bool isHiding = true;
    bool isMoving = false;

    private void Start()
    {
        transform.position = hiddenPosition.position;
    }

    public void Toggle()
    {  
        if(isHiding) { Show(); }
        else { Hide(); }
    }

    public void SetState(bool hidden)
    {
        if (hidden)
        {
            transform.position = hiddenPosition.position;
        }
        else
        {
            transform.position = shownPosition.position;
        }
    }

    public void Hide()
    {
        isMoving = true;
        isHiding = true;

        AudioManager.Instance.PlaySound(AudioData.Instance.UI_MenuClose, 0.5f);
    }

    public void Show()
    {
        isMoving = true;
        isHiding = false;

        AudioManager.Instance.PlaySound(AudioData.Instance.UI_MenuOpen, 0.5f);
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
