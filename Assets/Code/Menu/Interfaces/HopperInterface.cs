using Logistics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopperInterface : ModularInterface
{ 
    public SingleStackInventoryElement InventoryElement;

    private Hopper hopper;

    private StructureData structureData;

    public void Update()
    {
        Graphics.DrawMesh(GlobalData.Instance.m_TileGizmo, hopper.transform.ToMatrix(), GlobalData.Instance.mat_PulsingGizmo, 0);
    }

    private void FixedUpdate()
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

    public void Init(Hopper _hopper, Vector3 screenPosition)
    { 
        this.hopper = _hopper; 

        structureData = hopper.StructureData;

        SetDetails(structureData.sprite, structureData.screenname, structureData.description);

        transform.position = screenPosition;

        InventoryElement.inventory = hopper.storageInventory;
    }

    public override void OnCloseInterface()
    {
        base.OnCloseInterface();

        if (hopper != null) { hopper.OnInterfaceClosed(); }
    } 
}
