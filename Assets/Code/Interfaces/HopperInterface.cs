using Logistics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopperInterface : StaticInterface
{ 
    public SingleStackInventoryElement InventoryElement; 
    private Hopper hopper; 
    private StructureData structureData; 

    public override void Init(Entity entity, Vector3 screenPosition)
    {
        base.Init(entity, screenPosition);

        this.hopper = (Hopper)entity; 

        structureData = hopper.StructureData;

        SetDetails(structureData.sprite, structureData.screenname, structureData.description); 

        InventoryElement.inventory = hopper.storageInventory; 
    }

    public void Update()
    {
        //Graphics.DrawMesh(GlobalData.Instance.gizmo_TileGrid, hopper.transform.ToMatrix(), GlobalData.Instance.mat_PulsingGizmo, 0);
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
        hopper.requestResource = resourceData;
    }
}
