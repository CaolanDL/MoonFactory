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

        dropdownHandler.callbackAction += hopperInterface.SetRequestResource; 
    }

    private void Start()
    {
        dropdownHandler.Populate(GetResources());
        dropdownHandler.SetSelected(hopperInterface.hopper.RequestPort.TargetResource);
    }

    List<ResourceData> GetResources()
    {
        var resources = GameManager.Instance.ScienceManager.unlocked_Resources;
        resources.Insert(0, null);

        return GameManager.Instance.ScienceManager.unlocked_Resources; 
    }  
}
