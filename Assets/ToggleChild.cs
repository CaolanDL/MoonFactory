using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleChild : MonoBehaviour
{
    [SerializeField] GameObject child;

    public void Toggle()
    {
        child.SetActive(!child.activeSelf);
    }
}
