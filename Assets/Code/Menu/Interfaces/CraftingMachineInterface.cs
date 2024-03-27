using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineInterface : ModularInterface
{
    [Space]
    public GameObject InputInventoriesGrid;
    public GameObject OutputInventoriesGrid;

    public List<SingleStackInventoryElement> InputInvElements;
    public List<SingleStackInventoryElement> OutputInvElements;

    private Machine machine;

    private StructureData structureData;

    public void Update()
    {
        Graphics.DrawMesh(GlobalData.Instance.gizmo_Axis, machine.transform.ToMatrix(), GlobalData.Instance.mat_PulsingGizmo, 0);
    }

    public override void UpdateUI()
    {
        UpdateInputInventoryElements();
        UpdateOuputInventoryElements();
    }

    public override void Init(Entity entity, Vector3 screenPosition)
    { 
        this.machine = (Machine)entity; 

        structureData = machine.StructureData;

        SetDetails(structureData.sprite, structureData.screenname, structureData.description);

        transform.position = screenPosition;

        foreach (var inventory in machine.InputInventories)
        {
            var newElement = Instantiate(MenuData.Instance.InventoryElement, InputInventoriesGrid.transform);
            var newElementComponent = newElement.GetComponent<SingleStackInventoryElement>();
            InputInvElements.Add(newElementComponent);
            newElementComponent.inventory = inventory;
        }
        foreach (var inventory in machine.OutputInventories)
        {
            var newElement = Instantiate(MenuData.Instance.InventoryElement, OutputInventoriesGrid.transform);
            var newElementComponent = newElement.GetComponent<SingleStackInventoryElement>();
            OutputInvElements.Add(newElementComponent);
            newElementComponent.inventory = inventory;
        }
    }

    public override void OnCloseInterface()
    {
        base.OnCloseInterface();

        if (machine != null) { machine.OnInterfaceClosed(); }
    }

    public void UpdateInputInventoryElements()
    {
        for (var i = 0; i < InputInvElements.Count; i++)
        {
            if (machine.InputInventories[i].stacks.Count == 0) continue;

            InputInvElements[i].resourceStack = machine.InputInventories[i].stacks[0];
        }

        foreach (var element in InputInvElements)
        {
            element.UpdateDisplay();
        }
    }

    public void UpdateOuputInventoryElements()
    {
        for (var i = 0; i < OutputInvElements.Count; i++)
        {
            if (machine.OutputInventories[i].stacks.Count == 0) continue;

            OutputInvElements[i].resourceStack = machine.OutputInventories[i].stacks[0];
        }

        foreach (var element in OutputInvElements)
        {
            element.UpdateDisplay();
        }
    }
}
