using Logistics; 
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

        if (TutorialProxy.IsActive) TutorialProxy.Action?.Invoke(TutorialEvent.HopperInterfaceOpened);
    }

    public override void Update()
    {
        base.Update();

        if (TutorialProxy.IsActive)
        {
            TutorialProxy.SetPopupPosition?.Invoke(transform.position, TutorialTag.HopperInterfacePosition);
        }
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

        if (TutorialProxy.IsActive) TutorialProxy.Action?.Invoke(TutorialEvent.HopperInterfaceClosed);
    }

    public void SetRequestResource(ResourceData resourceData)
    {
        if (resourceData == null)
        {
            hopper.isRequestor = false;
            hopper.isSupplier = true;
            hopper.SupplyPort.enabled = true;
            hopper.RequestPort.SetRequest(null, 0);
        }
        else
        {
            hopper.RequestPort.SetRequest(resourceData, 16);
            hopper.isSupplier = false;
            hopper.SupplyPort.enabled = false;
            hopper.isRequestor = true;

            if (TutorialProxy.IsActive) TutorialProxy.Action?.Invoke(TutorialEvent.HopperDropdownItemSelected);
        }
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
