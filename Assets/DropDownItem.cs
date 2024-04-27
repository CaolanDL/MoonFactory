using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DropDownItem : MonoBehaviour
{
    [SerializeField] UnityEvent UnityEvent;

    private void OnMouseDown()
    {
        UnityEvent.Invoke();
    }
}
