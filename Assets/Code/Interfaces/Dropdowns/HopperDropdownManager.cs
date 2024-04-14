using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopperDropdownManager : MonoBehaviour
{
    ResourceDropdownHandler dropdownHandler;

    HopperInterface hopperInterface; 

    private void Awake()
    {
        dropdownHandler = GetComponent<ResourceDropdownHandler>();  
        hopperInterface = GetComponentInParent<HopperInterface>();

        dropdownHandler.SetCallback(hopperInterface.SetRequestResource);  
    }

    private void Start()
    {
        dropdownHandler.Populate(GetResources());
    }

    List<ResourceData> GetResources()
    { 
        return GameManager.Instance.GlobalData.unlocked_Resources; 
    }  
}
