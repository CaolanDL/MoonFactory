using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseButton : MonoBehaviour
{
    StaticInterface modularInterface;

    private void Awake()
    {
        modularInterface = GetComponentInParent<StaticInterface>();
    }

    public void ButtonPressed()
    {
        modularInterface.CloseInterface();
    }
}
