using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public class CraftingDropdownManager : MonoBehaviour
{
    ResourceDropdownHandler dropdownHandler;

    CraftingMachineInterface machineInterface;

    private void Awake()
    {
        dropdownHandler = GetComponent<ResourceDropdownHandler>();
        machineInterface = GetComponentInParent<CraftingMachineInterface>();

        dropdownHandler.SetCallback(machineInterface.SetCraftingResource);
    }

    private void Start()
    {
        dropdownHandler.Populate(machineInterface.GetCraftableResources());
    }
}
