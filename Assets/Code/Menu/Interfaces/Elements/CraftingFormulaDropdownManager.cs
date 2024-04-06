using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingFormulaDropdownManager : MonoBehaviour
{
    ResourceDropdownHandler dropdownHandler;

    CraftingMachineInterface machineInterface;

    private void Awake()
    {
        dropdownHandler = GetComponent<ResourceDropdownHandler>();
        machineInterface = GetComponentInParent<CraftingMachineInterface>();

        //dropdownHandler.SetCallback(machineInterface.SetCraftingFormula);
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
