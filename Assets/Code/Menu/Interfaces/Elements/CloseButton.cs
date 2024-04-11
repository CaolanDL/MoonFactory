using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseButton : MonoBehaviour
{
    ModularInterface modularInterface;

    private void Awake()
    {
        modularInterface = GetComponentInParent<ModularInterface>();
    }

    public void ButtonPressed()
    {
        modularInterface.CloseInterface();
    }
}
