using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class CraftingMachineInterface : StaticInterface
{
    [Space]
    public GameObject InputInventoriesGrid;
    public GameObject OutputInventoriesGrid;

    public List<SingleStackInventoryElement> InputInvElements;
    public List<SingleStackInventoryElement> OutputInvElements;

    [SerializeField] public Slider PowerMeter;

    public Machine machine;
    private StructureData structureData;


    public void SetCraftingResource(ResourceData resource)
    {
        machine.newCraftingResource = resource;
    }

    public List<ResourceData> GetCraftableResources()
    {
        return structureData.CraftableResources;
    }

    public override void Update()
    {
        base.Update();

        if (TutorialProxy.IsActive) TutorialProxy.SetPopupPosition(transform.position, TutorialTag.MachineInterfacePosition); 
    }

    public override void UpdateUI()
    {
        UpdateInputInventoryElements();
        UpdateOuputInventoryElements();
        UpdatePowerMeter();
    }

    public override void Init(Entity entity, Vector3 screenPosition)
    {
        base.Init(entity, screenPosition);

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

        if (TutorialProxy.IsActive) TutorialProxy.Action?.Invoke(TutorialEvent.MachineInterfaceOpened);
    }

    public override void OnCloseInterface()
    {
        base.OnCloseInterface();

        if (machine != null) { machine.OnInterfaceClosed(); }

        if (TutorialProxy.IsActive) TutorialProxy.Action?.Invoke(TutorialEvent.MachineInterfaceClosed);
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

    public void UpdatePowerMeter()
    {
        if (PowerMeter == null || machine == null) { return; }
        if (machine.ElectricalNode == null)
        {
            PowerMeter.value = 0;
            return;
        }
        if (machine.ElectricalNode.Network == null)
        {
            PowerMeter.value = 0;
            return;
        }
        else
        {
            PowerMeter.value = machine.ElectricalNode.Network.ClampedPowerRatio;
            return;
        } 
    }
}
