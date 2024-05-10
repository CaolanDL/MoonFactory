using Logistics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HopperInterface : StaticInterface
{ 
    public SingleStackInventoryElement InventoryElement;

    [SerializeField] Toggle requestToggle;
    [SerializeField] Toggle supplyToggle;

    public Hopper hopper; 
    private StructureData structureData;

    public override void Init(Entity entity, Vector3 screenPosition)
    {
        base.Init(entity, screenPosition);

        this.hopper = (Hopper)entity; 

        structureData = hopper.StructureData;

        SetDetails(structureData.sprite, structureData.screenname, structureData.description); 

        InventoryElement.inventory = hopper.storageInventory;

        requestToggle.SetIsOnWithoutNotify(hopper.isRequestor);
        supplyToggle.SetIsOnWithoutNotify(hopper.isSupplier);
    } 

    public override void UpdateUI()
    {
        UpdateInventoryElement();
    }

    public void UpdateInventoryElement()
    {
        var hopperInventory = hopper.OutputInventories[0];

        if (hopperInventory.stacks.Count == 0) 
        { 
            InventoryElement.resourceStack = null;  
        }
        else
        {
            InventoryElement.resourceStack = hopperInventory.stacks[0]; 
        }
        InventoryElement.UpdateDisplay(); 
    }

    public override void OnCloseInterface()
    {
        base.OnCloseInterface();

        if (hopper != null) { hopper.OnInterfaceClosed(); }
    } 

    public void SetRequestResource(ResourceData resourceData)
    {
        hopper.RequestPort.SetRequest(resourceData, 10);
    }

    public void SupplyToggleChanged(bool value)
    {
        hopper.isSupplier = value;
    }

    public void RequestToggleChanged(bool value)
    {
        hopper.isRequestor = value;
    }
}
