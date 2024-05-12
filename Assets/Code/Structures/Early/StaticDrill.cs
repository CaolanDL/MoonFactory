using ExtensionMethods;
using System.Collections;
using UnityEngine;


public class StaticDrill : Structure
{ 
    public Inventory inventory = new Inventory(); 
    ResourceData outputResource; 

    static int initialDelayTime = 25;
    int initialDelay = 0;
    static int miningDelay = 50; //1 second
    int miningCountdown = miningDelay;
    bool isMining;
     
    public override void OnInitialise()
    {
        base.OnInitialise();

        outputResource = StructureData.CraftableResources[0];

        inventory.maxItems = 10;

        SupplyPort = new SupplyPort(this);
        SupplyPort.AddInventory(inventory);

        ElectricalNode = new Electrical.Sink();
    }

    public override void OnConstructed()
    {
        base.OnConstructed();

        if (TutorialProxy.IsActive)
        {
            TutorialProxy.Action?.Invoke(TutorialEvent.StaticDrillBuilt);
        }
    }

    public override void OnFrameUpdate()
    {
        base.OnFrameUpdate();
        
        if(TutorialProxy.IsActive)
        {
            TutorialProxy.SetPopupPosition?.Invoke(GameManager.Instance.CameraController.activeMainCamera.WorldToScreenPoint(DisplayObject.transform.position), TutorialTag.StaticDrillPosition);
        }
    }

    public override void OnTick()
    {
        base.OnTick();
        if(initialDelay < initialDelayTime)
        {
            initialDelay++;
            return;
        }

        if (inventory.GetMaxAcceptable(outputResource) > 0)
        {
            if(!isMining) { isMining = true; StartedMining(); }

            miningCountdown--;

            if(miningCountdown <= 0)
            {
                miningCountdown = miningDelay;
                inventory.TryAddResource(outputResource, 1);
            }
        }
        else
        {
            isMining = false;
            StoppedMining();
        }
    }

    void StartedMining()
    {
        DisplayObject.SetLoopingAnimation("Drilling");
    }

    void StoppedMining()
    {
        DisplayObject.SetLoopingAnimation("Idle");
    }

    public override void OnClicked(Vector3 mousePosition)
    {
        OpenInterfaceOnHUD(MenuData.Instance.StaticDrillInterface, mousePosition);
    }
}